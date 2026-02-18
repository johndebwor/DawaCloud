using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.HR.Queries;

public record GetLeaveRequestsQuery(
    int? EmployeeId = null,
    LeaveStatus? Status = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null
) : IRequest<List<LeaveRequestDto>>;

public class LeaveRequestDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeCode { get; set; } = string.Empty;
    public string LeaveTypeName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalDays { get; set; }
    public LeaveStatus Status { get; set; }
    public string? Reason { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? RejectionReason { get; set; }
}

public class GetLeaveRequestsQueryHandler : IRequestHandler<GetLeaveRequestsQuery, List<LeaveRequestDto>>
{
    private readonly AppDbContext _context;

    public GetLeaveRequestsQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<LeaveRequestDto>> Handle(GetLeaveRequestsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.LeaveRequests
            .Include(lr => lr.Employee)
            .Include(lr => lr.LeaveType)
            .AsQueryable();

        if (request.EmployeeId.HasValue)
        {
            query = query.Where(lr => lr.EmployeeId == request.EmployeeId.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(lr => lr.Status == request.Status.Value);
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(lr => lr.StartDate >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(lr => lr.EndDate <= request.ToDate.Value);
        }

        return await query
            .OrderByDescending(lr => lr.CreatedAt)
            .Select(lr => new LeaveRequestDto
            {
                Id = lr.Id,
                EmployeeId = lr.EmployeeId,
                EmployeeName = lr.Employee.FirstName + " " + lr.Employee.LastName,
                EmployeeCode = lr.Employee.EmployeeCode,
                LeaveTypeName = lr.LeaveType.Name,
                StartDate = lr.StartDate,
                EndDate = lr.EndDate,
                TotalDays = lr.TotalDays,
                Status = lr.Status,
                Reason = lr.Reason,
                ApprovedBy = lr.ApprovedBy,
                ApprovedAt = lr.ApprovedAt,
                RejectionReason = lr.RejectionReason
            })
            .ToListAsync(cancellationToken);
    }
}
