using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.HR.Queries;

public record GetAttendanceQuery(
    int? EmployeeId = null,
    int? DepartmentId = null,
    DateTime? Date = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null
) : IRequest<List<AttendanceDto>>;

public class AttendanceDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeCode { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public DateTime? ClockIn { get; set; }
    public DateTime? ClockOut { get; set; }
    public double? HoursWorked { get; set; }
    public AttendanceStatus Status { get; set; }
    public string? Notes { get; set; }
}

public class GetAttendanceQueryHandler : IRequestHandler<GetAttendanceQuery, List<AttendanceDto>>
{
    private readonly AppDbContext _context;

    public GetAttendanceQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<AttendanceDto>> Handle(GetAttendanceQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Attendances
            .Include(a => a.Employee)
            .ThenInclude(e => e.Department)
            .AsQueryable();

        if (request.EmployeeId.HasValue)
        {
            query = query.Where(a => a.EmployeeId == request.EmployeeId.Value);
        }

        if (request.DepartmentId.HasValue)
        {
            query = query.Where(a => a.Employee.DepartmentId == request.DepartmentId.Value);
        }

        if (request.Date.HasValue)
        {
            query = query.Where(a => a.Date.Date == request.Date.Value.Date);
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(a => a.Date >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(a => a.Date <= request.ToDate.Value);
        }

        return await query
            .OrderByDescending(a => a.Date)
            .ThenBy(a => a.Employee.LastName)
            .Select(a => new AttendanceDto
            {
                Id = a.Id,
                EmployeeId = a.EmployeeId,
                EmployeeName = a.Employee.FirstName + " " + a.Employee.LastName,
                EmployeeCode = a.Employee.EmployeeCode,
                DepartmentName = a.Employee.Department.Name,
                Date = a.Date,
                ClockIn = a.ClockIn,
                ClockOut = a.ClockOut,
                HoursWorked = a.ClockOut.HasValue && a.ClockIn.HasValue
                    ? (a.ClockOut.Value - a.ClockIn.Value).TotalHours
                    : null,
                Status = a.Status,
                Notes = a.Notes
            })
            .ToListAsync(cancellationToken);
    }
}
