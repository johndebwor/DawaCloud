namespace DawaFlow.Web.Data.Entities;

public class Supplier : BaseAuditableEntity
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; } = "Kenya";
    public string? TaxId { get; set; }
    public bool EmailNotificationsEnabled { get; set; } = true;
    public bool WhatsAppNotificationsEnabled { get; set; } = true;
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }

    // Navigation
    public ICollection<SupplierContact> Contacts { get; set; } = new List<SupplierContact>();
}

public class SupplierContact : BaseAuditableEntity
{
    public int Id { get; set; }
    public int SupplierId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Position { get; set; }
    public bool IsPrimary { get; set; }

    // Navigation
    public Supplier Supplier { get; set; } = null!;
}
