namespace DawaCloud.Web.Data.Entities;

public enum EmploymentStatus
{
    Active,
    OnLeave,
    Suspended,
    Terminated
}

public enum EmploymentType
{
    FullTime,
    PartTime,
    Contract,
    Intern
}

public enum Gender
{
    Male,
    Female,
    Other
}

public class Employee : BaseAuditableEntity
{
    public int Id { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public Gender? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? NationalId { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }

    // Employment
    public int DepartmentId { get; set; }
    public string Position { get; set; } = string.Empty;
    public EmploymentType EmploymentType { get; set; } = EmploymentType.FullTime;
    public EmploymentStatus Status { get; set; } = EmploymentStatus.Active;
    public DateTime HireDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public decimal BasicSalary { get; set; }

    // Emergency Contact
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? EmergencyContactRelation { get; set; }

    public string? Notes { get; set; }

    // Navigation
    public Department Department { get; set; } = null!;
    public ICollection<Attendance> AttendanceRecords { get; set; } = new List<Attendance>();
    public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
}
