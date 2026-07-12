using Microsoft.AspNetCore.Mvc;
using ITAssetManagement.Services;

namespace ITAssetManagement.Controllers;

[ApiController]
[Route("api/audit")]
public class AuditController : ControllerBase
{
    private readonly MockDataService _data;

    public AuditController(MockDataService data)
    {
        _data = data;
    }

    [HttpGet]
    public IActionResult GetList(
        [FromQuery] string? action,
        [FromQuery] string? resourceType,
        [FromQuery] string? keyword)
    {
        var query = _data.AuditLogs.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(action))
        {
            query = query.Where(l => l.Action == action);
        }

        if (!string.IsNullOrWhiteSpace(resourceType))
        {
            query = query.Where(l => l.ResourceType == resourceType);
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var k = keyword.Trim();
            query = query.Where(l =>
                l.UserName.Contains(k, StringComparison.OrdinalIgnoreCase) ||
                l.Action.Contains(k, StringComparison.OrdinalIgnoreCase) ||
                l.ResourceType.Contains(k, StringComparison.OrdinalIgnoreCase) ||
                (!string.IsNullOrEmpty(l.Detail) && l.Detail.Contains(k, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(l.ResourceId) && l.ResourceId.Contains(k, StringComparison.OrdinalIgnoreCase)));
        }

        return Ok(query.OrderByDescending(l => l.Timestamp).ToList());
    }
}
