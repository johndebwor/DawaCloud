namespace DawaCloud.Web.Data.Entities;

public class DrugCategory : BaseAuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentId { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public DrugCategory? Parent { get; set; }
    public ICollection<DrugCategory> Children { get; set; } = new List<DrugCategory>();
    public ICollection<Drug> Drugs { get; set; } = new List<Drug>();
}
