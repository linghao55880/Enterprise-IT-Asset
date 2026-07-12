namespace ITAssetManagement.Models;

public class Asset
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8].ToUpper();
    public string AssetCode { get; set; } = "";
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public string SubCategory { get; set; } = "";
    public string Status { get; set; } = "在用";
    public string? IpAddress { get; set; }
    public string? ConfigInfo { get; set; }
    public string? OsVersion { get; set; }
    public string Purpose { get; set; } = "";
    public string? Location { get; set; }
    public string ResponsiblePerson { get; set; } = "";
    public DateTime? PurchaseDate { get; set; }
    public decimal? PurchasePrice { get; set; }
    public string? Supplier { get; set; }
    public DateTime? WarrantyExpire { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
