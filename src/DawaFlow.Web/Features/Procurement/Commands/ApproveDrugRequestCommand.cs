using DawaFlow.Web.Data;
using DawaFlow.Web.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaFlow.Web.Features.Procurement.Commands;

public record ApproveDrugRequestCommand(
    int RequestId,
    bool IsApproved,
    string? RejectionReason = null
) : IRequest<ApproveDrugRequestResult>;

public record ApproveDrugRequestResult(
    bool Success,
    string Message
);

public class ApproveDrugRequestCommandHandler : IRequestHandler<ApproveDrugRequestCommand, ApproveDrugRequestResult>
{
    private readonly AppDbContext _context;

    public ApproveDrugRequestCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ApproveDrugRequestResult> Handle(ApproveDrugRequestCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var drugRequest = await _context.DrugRequests
                .FirstOrDefaultAsync(dr => dr.Id == request.RequestId, cancellationToken);

            if (drugRequest == null)
            {
                return new ApproveDrugRequestResult(false, "Drug request not found");
            }

            if (drugRequest.Status != DrugRequestStatus.PendingApproval)
            {
                return new ApproveDrugRequestResult(false, "Drug request is not pending approval");
            }

            if (request.IsApproved)
            {
                drugRequest.Status = DrugRequestStatus.Approved;
                drugRequest.ApprovedAt = DateTime.UtcNow;
                drugRequest.ApprovedBy = drugRequest.UpdatedBy; // From audit
            }
            else
            {
                drugRequest.Status = DrugRequestStatus.Rejected;
                drugRequest.RejectionReason = request.RejectionReason;
            }

            await _context.SaveChangesAsync(cancellationToken);

            var message = request.IsApproved
                ? $"Drug request {drugRequest.RequestNumber} approved successfully"
                : $"Drug request {drugRequest.RequestNumber} rejected";

            return new ApproveDrugRequestResult(true, message);
        }
        catch (Exception ex)
        {
            return new ApproveDrugRequestResult(
                false,
                $"Error processing approval: {ex.Message}"
            );
        }
    }
}
