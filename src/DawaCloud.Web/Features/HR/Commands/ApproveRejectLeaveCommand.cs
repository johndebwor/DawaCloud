using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using DawaCloud.Web.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.HR.Commands;

public record ApproveRejectLeaveCommand(
    int Id,
    bool Approve,
    string? RejectionReason,
    string ApprovedByName
) : IRequest<Result>;

public class ApproveRejectLeaveCommandHandler : IRequestHandler<ApproveRejectLeaveCommand, Result>
{
    private readonly AppDbContext _context;

    public ApproveRejectLeaveCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(ApproveRejectLeaveCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var leaveRequest = await _context.LeaveRequests
                .FirstOrDefaultAsync(lr => lr.Id == request.Id, cancellationToken);

            if (leaveRequest == null)
                return Result.Fail("Leave request not found.");

            if (leaveRequest.Status != LeaveStatus.Pending)
                return Result.Fail("Only pending leave requests can be approved or rejected.");

            if (request.Approve)
            {
                leaveRequest.Status = LeaveStatus.Approved;
                leaveRequest.ApprovedBy = request.ApprovedByName;
                leaveRequest.ApprovedAt = DateTime.UtcNow;
            }
            else
            {
                leaveRequest.Status = LeaveStatus.Rejected;
                leaveRequest.RejectionReason = request.RejectionReason;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error processing leave request: {ex.Message}");
        }
    }
}
