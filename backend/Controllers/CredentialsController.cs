using Microsoft.AspNetCore.Mvc;
using ITAssetManagement.Models;
using ITAssetManagement.Services;

namespace ITAssetManagement.Controllers;

[ApiController]
[Route("api/credentials")]
public class CredentialsController : ControllerBase
{
    private readonly MockDataService _data;

    public CredentialsController(MockDataService data)
    {
        _data = data;
    }

    [HttpGet]
    public IActionResult GetList([FromQuery] string? group, [FromQuery] string? keyword)
    {
        var query = _data.Credentials.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(group))
        {
            query = query.Where(c => c.GroupName == group);
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var k = keyword.Trim();
            query = query.Where(c =>
                c.Title.Contains(k, StringComparison.OrdinalIgnoreCase) ||
                (!string.IsNullOrEmpty(c.Username) && c.Username.Contains(k, StringComparison.OrdinalIgnoreCase)) ||
                c.CredentialType.Contains(k, StringComparison.OrdinalIgnoreCase) ||
                (!string.IsNullOrEmpty(c.Url) && c.Url.Contains(k, StringComparison.OrdinalIgnoreCase)));
        }

        var result = query.OrderByDescending(c => c.CreatedAt).Select(c => new Credential
        {
            Id = c.Id,
            AssetId = c.AssetId,
            Title = c.Title,
            CredentialType = c.CredentialType,
            Username = c.Username,
            Password = MaskPassword(c.Password),
            Url = c.Url,
            Notes = c.Notes,
            GroupName = c.GroupName,
            ExpiresAt = c.ExpiresAt,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        }).ToList();

        return Ok(result);
    }

    [HttpGet("{id}")]
    public IActionResult GetDetail(string id)
    {
        var cred = _data.Credentials.FirstOrDefault(c => c.Id == id);
        if (cred == null)
        {
            return NotFound(new { message = "凭据不存在" });
        }

        return Ok(new Credential
        {
            Id = cred.Id,
            AssetId = cred.AssetId,
            Title = cred.Title,
            CredentialType = cred.CredentialType,
            Username = cred.Username,
            Password = MaskPassword(cred.Password),
            Url = cred.Url,
            Notes = cred.Notes,
            GroupName = cred.GroupName,
            ExpiresAt = cred.ExpiresAt,
            CreatedAt = cred.CreatedAt,
            UpdatedAt = cred.UpdatedAt
        });
    }

    public class RevealRequest
    {
        public string ConfirmPassword { get; set; } = "";
    }

    [HttpPost("{id}/reveal")]
    public IActionResult Reveal(string id, [FromBody] RevealRequest request, [FromQuery] string? userId = "U001", [FromQuery] string? userName = "系统管理员")
    {
        if (request.ConfirmPassword != "123456")
        {
            _data.AddAuditLog(userId ?? "", userName ?? "", "查看密码", "凭据", id, "失败", GetClientIp(), "密码校验失败");
            return Unauthorized(new { message = "密码校验失败" });
        }

        var cred = _data.Credentials.FirstOrDefault(c => c.Id == id);
        if (cred == null)
        {
            return NotFound(new { message = "凭据不存在" });
        }

        _data.AddAuditLog(userId ?? "", userName ?? "", "查看密码", "凭据", id, "成功", GetClientIp(), $"查看凭据密码: {cred.Title}");
        return Ok(new { password = cred.Password });
    }

    [HttpPost]
    public IActionResult Create([FromBody] Credential credential, [FromQuery] string? userId = "U001", [FromQuery] string? userName = "系统管理员")
    {
        credential.Id = Guid.NewGuid().ToString("N")[..8].ToUpper();
        credential.CreatedAt = DateTime.Now;
        credential.UpdatedAt = DateTime.Now;
        _data.Credentials.Add(credential);

        _data.AddAuditLog(userId ?? "", userName ?? "", "创建", "凭据", credential.Id, "成功", GetClientIp(), $"新增凭据: {credential.Title}");
        return Ok(credential);
    }

    [HttpPut("{id}")]
    public IActionResult Update(string id, [FromBody] Credential credential, [FromQuery] string? userId = "U001", [FromQuery] string? userName = "系统管理员")
    {
        var existing = _data.Credentials.FirstOrDefault(c => c.Id == id);
        if (existing == null)
        {
            return NotFound(new { message = "凭据不存在" });
        }

        existing.AssetId = credential.AssetId;
        existing.Title = credential.Title;
        existing.CredentialType = credential.CredentialType;
        existing.Username = credential.Username;
        if (!string.IsNullOrEmpty(credential.Password) && credential.Password != MaskPassword(existing.Password))
        {
            existing.Password = credential.Password;
        }
        existing.Url = credential.Url;
        existing.Notes = credential.Notes;
        existing.GroupName = credential.GroupName;
        existing.ExpiresAt = credential.ExpiresAt;
        existing.UpdatedAt = DateTime.Now;

        _data.AddAuditLog(userId ?? "", userName ?? "", "更新", "凭据", id, "成功", GetClientIp(), $"修改凭据: {credential.Title}");
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id, [FromQuery] string? userId = "U001", [FromQuery] string? userName = "系统管理员")
    {
        var existing = _data.Credentials.FirstOrDefault(c => c.Id == id);
        if (existing == null)
        {
            return NotFound(new { message = "凭据不存在" });
        }

        _data.Credentials.Remove(existing);
        _data.AddAuditLog(userId ?? "", userName ?? "", "删除", "凭据", id, "成功", GetClientIp(), $"删除凭据: {existing.Title}");
        return Ok(new { message = "删除成功" });
    }

    private static string MaskPassword(string password)
    {
        return new string('*', password.Length);
    }

    private string? GetClientIp()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }
}
