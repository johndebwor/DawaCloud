namespace DawaFlow.Web.Data.Entities;

public class StockMovement : BaseAuditableEntity
{
    public long Id { get; set; }
    public int DrugId { get; set; }
    public int BatchId { get; set; }
    public MovementType Type { get; set; }
    public int Quantity { get; set; }
    public int BalanceBefore { get; set; }
    public int BalanceAfter { get; set; }
    public string? Reference { get; set; }  // Sale#, GRN#, Adjustment#
    public string? Reason { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public Drug Drug { get; set; } = null!;
    public Batch Batch { get; set; } = null!;
}

public enum MovementType
{
    Purchase,
    Sale,
    Return,
    Adjustment,
    Transfer,
    WriteOff,
    Opening
}
