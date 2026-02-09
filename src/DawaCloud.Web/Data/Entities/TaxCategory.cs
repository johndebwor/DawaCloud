namespace DawaCloud.Web.Data.Entities;

public class TaxCategory : BaseAuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public string? Description { get; set; }
    public bool IsDefault { get; set; } = false;
    public bool IsActive { get; set; } = true;
}
