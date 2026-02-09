namespace DawaCloud.Web.Data.Entities;

public class RetailSale : BaseAuditableEntity
{
    public int Id { get; set; }
    public string ReceiptNumber { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; } = DateTime.UtcNow;
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }

    // Currency (retail sales are always in SSP base currency)
    public int? CurrencyId { get; set; }

    // All amounts in base currency (SSP)
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal ChangeAmount { get; set; }

    public RetailSaleStatus Status { get; set; } = RetailSaleStatus.Completed;
    public string? CashierId { get; set; }
    public string? ShiftId { get; set; }
    public bool HasPrescriptionItems { get; set; }
    public string? PrescriptionNumber { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public Currency? Currency { get; set; }
    public ICollection<RetailSaleItem> Items { get; set; } = new List<RetailSaleItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

public class RetailSaleItem : BaseAuditableEntity
{
    public int Id { get; set; }
    public int SaleId { get; set; }
    public int DrugId { get; set; }
    public int BatchId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal TaxRate { get; set; }
    public decimal LineTotal { get; set; }
    public bool IsPrescriptionItem { get; set; }
    
    public RetailSale Sale { get; set; } = null!;
    public Drug Drug { get; set; } = null!;
    public Batch Batch { get; set; } = null!;
}

public class HeldSale : BaseAuditableEntity
{
    public int Id { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string? CustomerName { get; set; }
    public string CashierId { get; set; } = string.Empty;
    public DateTime HeldAt { get; set; } = DateTime.UtcNow;
    public string ItemsJson { get; set; } = "[]";
    public decimal TotalAmount { get; set; }
    public bool IsRecalled { get; set; }
    public DateTime? RecalledAt { get; set; }
}

public class CashierShift : BaseAuditableEntity
{
    public int Id { get; set; }
    public string ShiftNumber { get; set; } = string.Empty;
    public string CashierId { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    // Currency (shifts are always in SSP base currency)
    public int? CurrencyId { get; set; }

    // All balances in base currency (SSP)
    public decimal OpeningBalance { get; set; }
    public decimal ClosingBalance { get; set; }
    public decimal ExpectedBalance { get; set; }
    public decimal Variance { get; set; }

    public ShiftStatus Status { get; set; } = ShiftStatus.Open;
    public string? Notes { get; set; }

    // Navigation
    public Currency? Currency { get; set; }
    public ICollection<RetailSale> Sales { get; set; } = new List<RetailSale>();
}

public enum RetailSaleStatus
{
    InProgress,
    Completed,
    Voided,
    Refunded
}

public enum ShiftStatus
{
    Open,
    Closed,
    Reconciled
}
