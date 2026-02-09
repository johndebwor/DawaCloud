namespace DawaCloud.Web.Data.Entities;

public class GoodsReceipt : BaseAuditableEntity
{
    public int Id { get; set; }
    public string GRNNumber { get; set; } = string.Empty;
    public int SupplierId { get; set; }
    public int? DrugRequestId { get; set; }
    public DateTime ReceivedDate { get; set; } = DateTime.UtcNow;
    public string? SupplierInvoiceNumber { get; set; }
    public string? DeliveryNoteNumber { get; set; }

    // Currency tracking (TotalAmount in original currency, TotalAmountBase in SSP)
    public int? CurrencyId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalAmountBase { get; set; }
    public decimal? ExchangeRateUsed { get; set; }

    public GoodsReceiptStatus Status { get; set; } = GoodsReceiptStatus.Draft;
    public string? ReceivedById { get; set; }
    public string? VerifiedById { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public string? Notes { get; set; }

    public Supplier Supplier { get; set; } = null!;
    public Currency? Currency { get; set; }
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

    // Cost in original currency (inherited from parent GoodsReceipt)
    public decimal UnitCost { get; set; }
    public decimal LineTotal => AcceptedQuantity * UnitCost;

    // Cost in base currency (SSP)
    public decimal UnitCostBase { get; set; }
    public decimal LineTotalBase => AcceptedQuantity * UnitCostBase;

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
