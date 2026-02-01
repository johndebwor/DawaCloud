namespace DawaFlow.Web.Data.Entities;

public class Batch : BaseAuditableEntity
{
    public int Id { get; set; }
    public int DrugId { get; set; }
    public string BatchNumber { get; set; } = string.Empty;
    public DateTime ManufactureDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public int InitialQuantity { get; set; }
    public int CurrentQuantity { get; set; }
    public int ReservedQuantity { get; set; }
    // Cost in base currency (SSP)
    public decimal CostPrice { get; set; }

    // Original purchase currency tracking
    public int? CostCurrencyId { get; set; }
    public decimal? CostPriceOriginal { get; set; }
    public decimal? ExchangeRateUsed { get; set; }

    public string? SupplierBatchRef { get; set; }
    public int? GoodsReceiptId { get; set; }
    public BatchStatus Status { get; set; } = BatchStatus.Active;

    // Navigation
    public Drug Drug { get; set; } = null!;
    public Currency? CostCurrency { get; set; }
}

public enum BatchStatus
{
    Active,
    Expired,
    Recalled,
    Quarantined,
    Depleted
}
