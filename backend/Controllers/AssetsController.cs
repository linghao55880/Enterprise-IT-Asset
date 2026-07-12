using Microsoft.AspNetCore.Mvc;
using ITAssetManagement.Models;
using ITAssetManagement.Services;

namespace ITAssetManagement.Controllers;

[ApiController]
[Route("api/assets")]
public class AssetsController : ControllerBase
{
    private readonly MockDataService _data;

    public AssetsController(MockDataService data)
    {
        _data = data;
    }

    [HttpGet]
    public IActionResult GetList([FromQuery] string? keyword)
    {
        var query = _data.Assets.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var k = keyword.Trim();
            query = query.Where(a =>
                a.Name.Contains(k, StringComparison.OrdinalIgnoreCase) ||
                (!string.IsNullOrEmpty(a.IpAddress) && a.IpAddress.Contains(k, StringComparison.OrdinalIgnoreCase)) ||
                a.Purpose.Contains(k, StringComparison.OrdinalIgnoreCase) ||
                a.AssetCode.Contains(k, StringComparison.OrdinalIgnoreCase));
        }

        return Ok(query.OrderByDescending(a => a.CreatedAt).ToList());
    }

    [HttpGet("{id}")]
    public IActionResult GetDetail(string id)
    {
        var asset = _data.Assets.FirstOrDefault(a => a.Id == id);
        if (asset == null)
        {
            return NotFound(new { message = "资产不存在" });
        }
        return Ok(asset);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Asset asset, [FromQuery] string? userId = "U001", [FromQuery] string? userName = "系统管理员")
    {
        asset.Id = Guid.NewGuid().ToString("N")[..8].ToUpper();
        asset.CreatedAt = DateTime.Now;
        asset.UpdatedAt = DateTime.Now;
        _data.Assets.Add(asset);

        _data.AddAuditLog(userId ?? "", userName ?? "", "创建", "资产", asset.Id, "成功", GetClientIp(), $"新增资产: {asset.Name}");
        return Ok(asset);
    }

    [HttpPut("{id}")]
    public IActionResult Update(string id, [FromBody] Asset asset, [FromQuery] string? userId = "U001", [FromQuery] string? userName = "系统管理员")
    {
        var existing = _data.Assets.FirstOrDefault(a => a.Id == id);
        if (existing == null)
        {
            return NotFound(new { message = "资产不存在" });
        }

        existing.AssetCode = asset.AssetCode;
        existing.Name = asset.Name;
        existing.Category = asset.Category;
        existing.SubCategory = asset.SubCategory;
        existing.Status = asset.Status;
        existing.IpAddress = asset.IpAddress;
        existing.ConfigInfo = asset.ConfigInfo;
        existing.OsVersion = asset.OsVersion;
        existing.Purpose = asset.Purpose;
        existing.Location = asset.Location;
        existing.ResponsiblePerson = asset.ResponsiblePerson;
        existing.PurchaseDate = asset.PurchaseDate;
        existing.PurchasePrice = asset.PurchasePrice;
        existing.Supplier = asset.Supplier;
        existing.WarrantyExpire = asset.WarrantyExpire;
        existing.Notes = asset.Notes;
        existing.UpdatedAt = DateTime.Now;

        _data.AddAuditLog(userId ?? "", userName ?? "", "更新", "资产", id, "成功", GetClientIp(), $"修改资产: {asset.Name}");
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id, [FromQuery] string? userId = "U001", [FromQuery] string? userName = "系统管理员")
    {
        var existing = _data.Assets.FirstOrDefault(a => a.Id == id);
        if (existing == null)
        {
            return NotFound(new { message = "资产不存在" });
        }

        _data.Assets.Remove(existing);
        _data.AddAuditLog(userId ?? "", userName ?? "", "删除", "资产", id, "成功", GetClientIp(), $"删除资产: {existing.Name}");
        return Ok(new { message = "删除成功" });
    }

    private string? GetClientIp()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }
}
