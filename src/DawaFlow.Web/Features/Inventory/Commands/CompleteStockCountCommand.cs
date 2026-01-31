using DawaFlow.Web.Data;
using DawaFlow.Web.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaFlow.Web.Features.Inventory.Commands;

public record CompleteStockCountCommand(
    int StockCountId,
    bool AutoAdjust = false // Automatically create adjustments for variances
) : IRequest<CompleteStockCountResult>;

public record CompleteStockCountResult(
    bool Success,
    string Message,
    int AdjustmentsCreated = 0
);

public class CompleteStockCountCommandHandler : IRequestHandler<CompleteStockCountCommand, CompleteStockCountResult>
{
    private readonly AppDbContext _context;

    public CompleteStockCountCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CompleteStockCountResult> Handle(CompleteStockCountCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var stockCount = await _context.StockCounts
                .Include(sc => sc.Items)
                .ThenInclude(sci => sci.Batch)
                .FirstOrDefaultAsync(sc => sc.Id == request.StockCountId, cancellationToken);

            if (stockCount == null)
            {
                return new CompleteStockCountResult(false, "Stock count not found");
            }

            if (stockCount.Status != StockCountStatus.InProgress)
            {
                return new CompleteStockCountResult(false, "Stock count is not in progress");
            }

            // Calculate summary
            var countedItems = stockCount.Items.Where(i => i.CountedAt.HasValue).ToList();
            stockCount.TotalItemsCounted = countedItems.Count;
            stockCount.ItemsWithVariance = countedItems.Count(i => i.Variance != 0);
            stockCount.TotalVarianceValue = countedItems.Sum(i => i.VarianceValue);

            // Mark as completed
            stockCount.Status = StockCountStatus.Completed;
            stockCount.CompletedAt = DateTime.UtcNow;
            stockCount.CompletedBy = stockCount.UpdatedBy; // From audit

            int adjustmentsCreated = 0;

            // Auto-adjust if requested
            if (request.AutoAdjust)
            {
                var itemsWithVariance = countedItems.Where(i => i.Variance != 0).ToList();

                foreach (var item in itemsWithVariance)
                {
                    // Update batch quantity
                    var batch = item.Batch;
                    var oldQuantity = batch.CurrentQuantity;
                    batch.CurrentQuantity = item.PhysicalQuantity;

                    // Create stock movement
                    var movement = new StockMovement
                    {
                        DrugId = item.DrugId,
                        BatchId = item.BatchId,
                        Type = MovementType.Adjustment,
                        Quantity = item.Variance,
                        BalanceBefore = oldQuantity,
                        BalanceAfter = item.PhysicalQuantity,
                        Reference = stockCount.ReferenceNumber,
                        Reason = "Stock count adjustment",
                        Notes = $"Auto-adjusted from stock count. Variance: {item.Variance:+0;-0;0} units"
                    };

                    _context.StockMovements.Add(movement);
                    adjustmentsCreated++;

                    // Mark batch as depleted if quantity is zero
                    if (batch.CurrentQuantity == 0 && batch.Status == BatchStatus.Active)
                    {
                        batch.Status = BatchStatus.Depleted;
                    }
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            var message = $"Stock count completed. {stockCount.TotalItemsCounted} items counted, {stockCount.ItemsWithVariance} with variance.";
            if (adjustmentsCreated > 0)
            {
                message += $" {adjustmentsCreated} automatic adjustments created.";
            }

            return new CompleteStockCountResult(true, message, adjustmentsCreated);
        }
        catch (Exception ex)
        {
            return new CompleteStockCountResult(
                false,
                $"Error completing stock count: {ex.Message}"
            );
        }
    }
}
