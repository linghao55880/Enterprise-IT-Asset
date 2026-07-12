namespace ITAssetManagement.Models;

public class Credential
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8].ToUpper();
    public string? AssetId { get; set; }
    public string Title { get; set; } = "";
    public string CredentialType { get; set; } = "";
    public string? Username { get; set; }
    public string Password { get; set; } = "";
    public string? Url { get; set; }
    public string? Notes { get; set; }
    public string GroupName { get; set; } = "";
    public DateTime? ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
