using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using DawaCloud.Web.Shared;
using MediatR;

namespace DawaCloud.Web.Features.HR.Commands;

public record CreateLeaveRequestCommand(
    int EmployeeId,
    int LeaveTypeId,
    DateTime StartDate,
    DateTime EndDate,
    string? Reason
) : IRequest<Result<int>>;

public class CreateLeaveRequestCommandHandler : IRequestHandler<CreateLeaveRequestCommand, Result<int>>
{
    private readonly AppDbContext _context;

    public CreateLeaveRequestCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(CreateLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var totalDays = (int)(request.EndDate.Date - request.StartDate.Date).TotalDays + 1;

            var leaveRequest = new LeaveRequest
            {
                EmployeeId = request.EmployeeId,
                LeaveTypeId = request.LeaveTypeId,
                StartDate = request.StartDate.Date,
                EndDate = request.EndDate.Date,
                TotalDays = totalDays,
                Reason = request.Reason,
                Status = LeaveStatus.Pending
            };

            _context.LeaveRequests.Add(leaveRequest);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<int>.Ok(leaveRequest.Id);
        }
        catch (Exception ex)
        {
            return Result<int>.Fail($"Error creating leave request: {ex.Message}");
        }
    }
}
