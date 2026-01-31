namespace DawaFlow.Web.Data.Entities;

public class GoodsReceipt : BaseAuditableEntity
{
    public int Id { get; set; }
    public string GRNNumber { get; set; } = string.Empty;
    public int SupplierId { get; set; }
    public int? DrugRequestId { get; set; }
    public DateTime ReceivedDate { get; set; } = DateTime.UtcNow;
    public string? SupplierInvoiceNumber { get; set; }
    public string? DeliveryNoteNumber { get; set; }
    public decimal TotalAmount { get; set; }
    public GoodsReceiptStatus Status { get; set; } = GoodsReceiptStatus.Draft;
    public string? ReceivedById { get; set; }
    public string? VerifiedById { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public string? Notes { get; set; }
    
    public Supplier Supplier { get; set; } = null!;
    public DrugRequest? DrugRequest { get; set; }
    public ICollection<GoodsReceiptItem> Items { get; set; } = new List<GoodsReceiptItem>();
}

public class GoodsReceiptItem : BaseAuditableEntity
{
    public int Id { get; set; }
    public int GoodsReceiptId { get; set; }
    public int DrugId { get; set; }
    public int? BatchId { get; set; }
    public string? BatchNumber { get; set; }
    public DateTime? ManufactureDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int OrderedQuantity { get; set; }
    public int ReceivedQuantity { get; set; }
    public int AcceptedQuantity { get; set; }
    public int RejectedQuantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal LineTotal => AcceptedQuantity * UnitCost;
    public string? RejectionReason { get; set; }
    
    public GoodsReceipt GoodsReceipt { get; set; } = null!;
    public Drug Drug { get; set; } = null!;
    public Batch? Batch { get; set; }
}

public enum GoodsReceiptStatus
{
    Draft,
    Pending,
    PartiallyVerified,
    Verified,
    Posted,
    Cancelled
}

public class CompanySettings
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; } = "Kenya";
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? TaxId { get; set; }
    public string? PharmacyLicenseNumber { get; set; }
    public string Currency { get; set; } = "KES";
    public decimal DefaultTaxRate { get; set; } = 16; // VAT
    public int LowStockThresholdDays { get; set; } = 14;
    public int ExpiryAlertDays30 { get; set; } = 30;
    public int ExpiryAlertDays60 { get; set; } = 60;
    public int ExpiryAlertDays90 { get; set; } = 90;
}
