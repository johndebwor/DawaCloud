using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.Inventory.Commands;

public record StartStockCountCommand(
    string? Notes = null
) : IRequest<StartStockCountResult>;

public record StartStockCountResult(
    bool Success,
    string Message,
    int? StockCountId = null
);

public class StartStockCountCommandHandler : IRequestHandler<StartStockCountCommand, StartStockCountResult>
{
    private readonly AppDbContext _context;

    public StartStockCountCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<StartStockCountResult> Handle(StartStockCountCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Generate reference number
            var today = DateTime.UtcNow;
            var refNumber = $"SC-{today:yyyyMMdd}-{GenerateSequence()}";

            // Create stock count
            var stockCount = new StockCount
            {
                ReferenceNumber = refNumber,
                CountDate = today,
                Status = StockCountStatus.InProgress,
                Notes = request.Notes
            };

            // Get all active batches and create count items
            var activeBatches = await _context.Batches
                .Include(b => b.Drug)
                .Where(b => b.Status == BatchStatus.Active)
                .ToListAsync(cancellationToken);

            foreach (var batch in activeBatches)
            {
                var countItem = new StockCountItem
                {
                    DrugId = batch.DrugId,
                    BatchId = batch.Id,
                    SystemQuantity = batch.CurrentQuantity,
                    PhysicalQuantity = 0, // To be counted
                    Variance = 0,
                    VarianceValue = 0
                };

                stockCount.Items.Add(countItem);
            }

            stockCount.TotalItemsCounted = 0;
            stockCount.ItemsWithVariance = 0;
            stockCount.TotalVarianceValue = 0;

            _context.StockCounts.Add(stockCount);
            await _context.SaveChangesAsync(cancellationToken);

            return new StartStockCountResult(
                true,
                $"Stock count {refNumber} started with {activeBatches.Count} items",
                stockCount.Id
            );
        }
        catch (Exception ex)
        {
            return new StartStockCountResult(
                false,
                $"Error starting stock count: {ex.Message}"
            );
        }
    }

    private static string GenerateSequence()
    {
        return Random.Shared.Next(1000, 9999).ToString();
    }
}
