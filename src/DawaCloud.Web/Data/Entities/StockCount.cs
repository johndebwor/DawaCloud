namespace DawaCloud.Web.Data.Entities;

public class StockCount : BaseAuditableEntity
{
    public int Id { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public DateTime CountDate { get; set; }
    public StockCountStatus Status { get; set; } = StockCountStatus.InProgress;
    public string? Notes { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? CompletedBy { get; set; }

    // Summary
    public int TotalItemsCounted { get; set; }
    public int ItemsWithVariance { get; set; }
    public decimal TotalVarianceValue { get; set; }

    // Navigation
    public ICollection<StockCountItem> Items { get; set; } = new List<StockCountItem>();
}

public class StockCountItem : BaseAuditableEntity
{
    public int Id { get; set; }
    public int StockCountId { get; set; }
    public int DrugId { get; set; }
    public int BatchId { get; set; }

    // System values at time of count
    public int SystemQuantity { get; set; }

    // Counted values
    public int PhysicalQuantity { get; set; }
    public int Variance { get; set; }  // Physical - System
    public decimal VarianceValue { get; set; }  // Variance * Cost

    public string? CountedBy { get; set; }
    public DateTime? CountedAt { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public StockCount StockCount { get; set; } = null!;
    public Drug Drug { get; set; } = null!;
    public Batch Batch { get; set; } = null!;
}

public enum StockCountStatus
{
    InProgress,
    Completed,
    Cancelled
}
