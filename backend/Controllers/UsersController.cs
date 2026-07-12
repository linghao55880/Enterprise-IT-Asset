using Microsoft.AspNetCore.Mvc;
using ITAssetManagement.Models;
using ITAssetManagement.Services;

namespace ITAssetManagement.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly MockDataService _data;

    public UsersController(MockDataService data)
    {
        _data = data;
    }

    [HttpGet]
    public IActionResult GetList()
    {
        var users = _data.Users.OrderBy(u => u.CreatedAt).Select(u => new User
        {
            Id = u.Id,
            Username = u.Username,
            DisplayName = u.DisplayName,
            Email = u.Email,
            Phone = u.Phone,
            Role = u.Role,
            Department = u.Department,
            IsActive = u.IsActive,
            LastLogin = u.LastLogin,
            CreatedAt = u.CreatedAt
        }).ToList();

        return Ok(users);
    }

    [HttpPost]
    public IActionResult Create([FromBody] User user, [FromQuery] string? operatorId = "U001", [FromQuery] string? operatorName = "系统管理员")
    {
        if (_data.Users.Any(u => u.Username == user.Username))
        {
            return BadRequest(new { message = "用户名已存在" });
        }

        user.Id = Guid.NewGuid().ToString("N")[..8].ToUpper();
        user.CreatedAt = DateTime.Now;
        user.LastLogin = null;
        _data.Users.Add(user);

        _data.AddAuditLog(operatorId ?? "", operatorName ?? "", "创建", "用户", user.Id, "成功", GetClientIp(), $"创建新用户: {user.DisplayName}");
        return Ok(user);
    }

    [HttpPut("{id}")]
    public IActionResult Update(string id, [FromBody] User user, [FromQuery] string? operatorId = "U001", [FromQuery] string? operatorName = "系统管理员")
    {
        var existing = _data.Users.FirstOrDefault(u => u.Id == id);
        if (existing == null)
        {
            return NotFound(new { message = "用户不存在" });
        }

        if (_data.Users.Any(u => u.Id != id && u.Username == user.Username))
        {
            return BadRequest(new { message = "用户名已存在" });
        }

        existing.Username = user.Username;
        existing.DisplayName = user.DisplayName;
        existing.Email = user.Email;
        existing.Phone = user.Phone;
        existing.Role = user.Role;
        existing.Department = user.Department;
        existing.IsActive = user.IsActive;

        _data.AddAuditLog(operatorId ?? "", operatorName ?? "", "更新", "用户", id, "成功", GetClientIp(), $"修改用户信息: {user.DisplayName}");
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id, [FromQuery] string? operatorId = "U001", [FromQuery] string? operatorName = "系统管理员")
    {
        var existing = _data.Users.FirstOrDefault(u => u.Id == id);
        if (existing == null)
        {
            return NotFound(new { message = "用户不存在" });
        }

        _data.Users.Remove(existing);
        _data.AddAuditLog(operatorId ?? "", operatorName ?? "", "删除", "用户", id, "成功", GetClientIp(), $"删除用户: {existing.DisplayName}");
        return Ok(new { message = "删除成功" });
    }

    private string? GetClientIp()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }
}
