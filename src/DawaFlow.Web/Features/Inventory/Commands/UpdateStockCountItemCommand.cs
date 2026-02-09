using DawaFlow.Web.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaFlow.Web.Features.Inventory.Commands;

public record UpdateStockCountItemCommand(
    int StockCountItemId,
    int PhysicalQuantity,
    string? Notes = null
) : IRequest<UpdateStockCountItemResult>;

public record UpdateStockCountItemResult(
    bool Success,
    string Message,
    int Variance = 0,
    decimal VarianceValue = 0
);

public class UpdateStockCountItemCommandHandler : IRequestHandler<UpdateStockCountItemCommand, UpdateStockCountItemResult>
{
    private readonly AppDbContext _context;

    public UpdateStockCountItemCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UpdateStockCountItemResult> Handle(UpdateStockCountItemCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var item = await _context.StockCountItems
                .Include(sci => sci.StockCount)
                .Include(sci => sci.Batch)
                .ThenInclude(b => b.Drug)
                .FirstOrDefaultAsync(sci => sci.Id == request.StockCountItemId, cancellationToken);

            if (item == null)
            {
                return new UpdateStockCountItemResult(false, "Stock count item not found");
            }

            if (item.StockCount.Status != Data.Entities.StockCountStatus.InProgress)
            {
                return new UpdateStockCountItemResult(false, "Stock count is not in progress");
            }

            // Update physical quantity
            item.PhysicalQuantity = request.PhysicalQuantity;
            item.Variance = request.PhysicalQuantity - item.SystemQuantity;

            // Calculate variance value (variance * cost price)
            var costPrice = item.Batch.CostPrice;
            item.VarianceValue = item.Variance * costPrice;

            // Mark as counted
            item.CountedAt = DateTime.UtcNow;
            item.CountedBy = item.StockCount.UpdatedBy; // From audit
            item.Notes = request.Notes;

            await _context.SaveChangesAsync(cancellationToken);

            return new UpdateStockCountItemResult(
                true,
                "Item counted successfully",
                item.Variance,
                item.VarianceValue
            );
        }
        catch (Exception ex)
        {
            return new UpdateStockCountItemResult(
                false,
                $"Error updating stock count item: {ex.Message}"
            );
        }
    }
}
