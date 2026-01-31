using DawaFlow.Web.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaFlow.Web.Features.Inventory.Commands;

public record RecordCountCommand(
    int StockCountItemId,
    int PhysicalQuantity,
    string? Notes = null
) : IRequest<RecordCountResult>;

public record RecordCountResult(
    bool Success,
    string Message
);

public class RecordCountCommandHandler : IRequestHandler<RecordCountCommand, RecordCountResult>
{
    private readonly AppDbContext _context;

    public RecordCountCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<RecordCountResult> Handle(RecordCountCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var countItem = await _context.StockCountItems
                .Include(sci => sci.Batch)
                .Include(sci => sci.StockCount)
                .FirstOrDefaultAsync(sci => sci.Id == request.StockCountItemId, cancellationToken);

            if (countItem == null)
            {
                return new RecordCountResult(false, "Stock count item not found");
            }

            if (countItem.StockCount.Status != Data.Entities.StockCountStatus.InProgress)
            {
                return new RecordCountResult(false, "Stock count is not in progress");
            }

            // Update count item
            countItem.PhysicalQuantity = request.PhysicalQuantity;
            countItem.Variance = request.PhysicalQuantity - countItem.SystemQuantity;
            countItem.VarianceValue = countItem.Variance * countItem.Batch.CostPrice;
            countItem.CountedBy = countItem.UpdatedBy; // Set from audit
            countItem.CountedAt = DateTime.UtcNow;
            if (!string.IsNullOrWhiteSpace(request.Notes))
            {
                countItem.Notes = request.Notes;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new RecordCountResult(
                true,
                $"Count recorded: {request.PhysicalQuantity} units (Variance: {countItem.Variance:+0;-0;0})"
            );
        }
        catch (Exception ex)
        {
            return new RecordCountResult(
                false,
                $"Error recording count: {ex.Message}"
            );
        }
    }
}
