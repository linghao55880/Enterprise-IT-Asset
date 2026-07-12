using Microsoft.AspNetCore.Mvc;
using ITAssetManagement.Models;
using ITAssetManagement.Services;

namespace ITAssetManagement.Controllers;

[ApiController]
[Route("api/approvals")]
public class ApprovalsController : ControllerBase
{
    private readonly MockDataService _data;

    public ApprovalsController(MockDataService data)
    {
        _data = data;
    }

    [HttpGet]
    public IActionResult GetList([FromQuery] string? status)
    {
        var query = _data.Approvals.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(a => a.Status == status);
        }

        return Ok(query.OrderByDescending(a => a.CreatedAt).ToList());
    }

    [HttpPost]
    public IActionResult Create([FromBody] ApprovalRequest request, [FromQuery] string? userId = "U001", [FromQuery] string? userName = "系统管理员")
    {
        var approval = new ApprovalRequest
        {
            Id = Guid.NewGuid().ToString("N")[..8].ToUpper(),
            RequestType = request.RequestType,
            RequesterId = request.RequesterId,
            RequesterName = request.RequesterName,
            TargetId = request.TargetId,
            TargetName = request.TargetName,
            Reason = request.Reason,
            Status = "待审批",
            CreatedAt = DateTime.Now
        };

        _data.Approvals.Add(approval);
        _data.AddAuditLog(userId ?? "", userName ?? "", "创建", "审批申请", approval.Id, "成功", GetClientIp(), $"提交{approval.RequestType}申请");
        return Ok(approval);
    }

    [HttpPost("{id}/approve")]
    public IActionResult Approve(string id, [FromQuery] string? userId = "U001", [FromQuery] string? userName = "系统管理员")
    {
        var approval = _data.Approvals.FirstOrDefault(a => a.Id == id);
        if (approval == null)
        {
            return NotFound(new { message = "审批记录不存在" });
        }

        if (approval.Status != "待审批")
        {
            return BadRequest(new { message = "该申请已处理" });
        }

        approval.Status = "已通过";
        approval.ApproverId = userId;
        approval.ApproverName = userName;
        approval.ApprovedAt = DateTime.Now;

        _data.AddAuditLog(userId ?? "", userName ?? "", "审批通过", "审批申请", id, "成功", GetClientIp(), $"审批通过{approval.RequestType}申请");
        return Ok(approval);
    }

    public class RejectRequest
    {
        public string Comment { get; set; } = "";
    }

    [HttpPost("{id}/reject")]
    public IActionResult Reject(string id, [FromBody] RejectRequest request, [FromQuery] string? userId = "U001", [FromQuery] string? userName = "系统管理员")
    {
        var approval = _data.Approvals.FirstOrDefault(a => a.Id == id);
        if (approval == null)
        {
            return NotFound(new { message = "审批记录不存在" });
        }

        if (approval.Status != "待审批")
        {
            return BadRequest(new { message = "该申请已处理" });
        }

        approval.Status = "已拒绝";
        approval.ApproverId = userId;
        approval.ApproverName = userName;
        approval.ApprovalComment = request.Comment;
        approval.ApprovedAt = DateTime.Now;

        _data.AddAuditLog(userId ?? "", userName ?? "", "审批拒绝", "审批申请", id, "成功", GetClientIp(), $"拒绝{approval.RequestType}申请: {request.Comment}");
        return Ok(approval);
    }

    private string? GetClientIp()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }
}
