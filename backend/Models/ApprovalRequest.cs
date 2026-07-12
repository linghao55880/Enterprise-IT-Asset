namespace ITAssetManagement.Models;

public class ApprovalRequest
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8].ToUpper();
    public string RequestType { get; set; } = ""; // 密码访问、资产报废、权限提升
    public string RequesterId { get; set; } = "";
    public string RequesterName { get; set; } = "";
    public string? TargetId { get; set; }
    public string? TargetName { get; set; }
    public string Reason { get; set; } = "";
    public string Status { get; set; } = "待审批"; // 待审批、已通过、已拒绝、已取消
    public string? ApproverId { get; set; }
    public string? ApproverName { get; set; }
    public string? ApprovalComment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? ApprovedAt { get; set; }
}
