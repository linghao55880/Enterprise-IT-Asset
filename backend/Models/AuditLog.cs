namespace ITAssetManagement.Models;

public class AuditLog
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8].ToUpper();
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string UserId { get; set; } = "";
    public string UserName { get; set; } = "";
    public string Action { get; set; } = "";
    public string ResourceType { get; set; } = "";
    public string? ResourceId { get; set; }
    public string Result { get; set; } = "成功";
    public string? SourceIp { get; set; }
    public string? Detail { get; set; }
}
