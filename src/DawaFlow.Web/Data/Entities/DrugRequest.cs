namespace DawaFlow.Web.Data.Entities;

public class DrugRequest : BaseAuditableEntity
{
    public int Id { get; set; }
    public string RequestNumber { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
    public int SupplierId { get; set; }
    public DrugRequestStatus Status { get; set; } = DrugRequestStatus.Draft;
    public string? Notes { get; set; }

    // Approval
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? RejectionReason { get; set; }

    // Supplier Response
    public DateTime? SupplierRespondedAt { get; set; }
    public string? SupplierNotes { get; set; }

    // Delivery
    public DateTime? ExpectedDeliveryDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }

    // Financial
    public decimal TotalAmount { get; set; }
    public decimal? ActualAmount { get; set; }

    // Navigation
    public Supplier Supplier { get; set; } = null!;
    public ICollection<DrugRequestItem> Items { get; set; } = new List<DrugRequestItem>();
}

public class DrugRequestItem : BaseAuditableEntity
{
    public int Id { get; set; }
    public int DrugRequestId { get; set; }
    public int DrugId { get; set; }
    public int RequestedQuantity { get; set; }
    public int? SuppliedQuantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string? Notes { get; set; }

    // Supplier response
    public bool? IsAvailable { get; set; }
    public decimal? QuotedPrice { get; set; }
    public string? SupplierNotes { get; set; }

    // Navigation
    public DrugRequest DrugRequest { get; set; } = null!;
    public Drug Drug { get; set; } = null!;
}

public enum DrugRequestStatus
{
    Draft,              // Being created
    PendingApproval,    // Submitted for approval
    Approved,           // Manager approved
    Rejected,           // Manager rejected
    SentToSupplier,     // Notification sent
    SupplierConfirmed,  // Supplier accepted
    SupplierRejected,   // Supplier declined
    PartiallyDelivered, // Some items delivered
    Delivered,          // All items delivered
    Completed,          // Stock posted
    Cancelled           // Cancelled by user
}
