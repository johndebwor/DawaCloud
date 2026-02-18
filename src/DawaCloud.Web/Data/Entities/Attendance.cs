namespace DawaCloud.Web.Data.Entities;

public enum AttendanceStatus
{
    Present,
    Absent,
    Late,
    HalfDay,
    OnLeave
}

public class Attendance : BaseAuditableEntity
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateTime Date { get; set; }
    public DateTime? ClockIn { get; set; }
    public DateTime? ClockOut { get; set; }
    public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;
    public string? Notes { get; set; }

    // Navigation
    public Employee Employee { get; set; } = null!;
}
