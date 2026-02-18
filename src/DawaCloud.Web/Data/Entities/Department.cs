namespace DawaCloud.Web.Data.Entities;

public class Department : BaseAuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? HeadEmployeeId { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public Employee? HeadEmployee { get; set; }
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
