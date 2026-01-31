namespace DawaFlow.Web.Data.Entities;

public class Drug : BaseAuditableEntity
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string GenericName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public string? Manufacturer { get; set; }
    public string? DosageForm { get; set; }
    public string? Strength { get; set; }
    public string? PackSize { get; set; }
    public decimal RetailPrice { get; set; }
    public decimal WholesalePrice { get; set; }
    public decimal CostPrice { get; set; }
    public int ReorderLevel { get; set; }
    public int MaxStockLevel { get; set; }
    public decimal TaxRate { get; set; } = 16; // Default VAT in Kenya
    public bool RequiresPrescription { get; set; }
    public bool IsControlled { get; set; }
    public bool IsActive { get; set; } = true;
    public string? ImageUrl { get; set; }

    // Navigation
    public DrugCategory Category { get; set; } = null!;
    public ICollection<Batch> Batches { get; set; } = new List<Batch>();
}
