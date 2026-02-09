using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.Inventory.Commands;

public class AdjustStockCommandHandler : IRequestHandler<AdjustStockCommand, AdjustStockResult>
{
    private readonly AppDbContext _context;

    public AdjustStockCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AdjustStockResult> Handle(AdjustStockCommand request, CancellationToken cancellationToken)
    {
        // Get the batch
        var batch = await _context.Batches
            .Include(b => b.Drug)
            .FirstOrDefaultAsync(b => b.Id == request.BatchId, cancellationToken);

        if (batch == null)
        {
            return new AdjustStockResult(false, "Batch not found");
        }

        // Validate adjustment
        var newQuantity = batch.CurrentQuantity + request.QuantityChange;
        if (newQuantity < 0)
        {
            return new AdjustStockResult(false, $"Cannot adjust below zero. Current quantity: {batch.CurrentQuantity}, Adjustment: {request.QuantityChange}");
        }

        if (newQuantity < batch.ReservedQuantity)
        {
            return new AdjustStockResult(false, $"Cannot adjust below reserved quantity of {batch.ReservedQuantity}");
        }

        // Record the balance before adjustment
        var balanceBefore = batch.CurrentQuantity;

        // Update batch quantity
        batch.CurrentQuantity = newQuantity;

        // If quantity reaches zero, mark batch as depleted
        if (batch.CurrentQuantity == 0 && batch.Status == BatchStatus.Active)
        {
            batch.Status = BatchStatus.Depleted;
        }

        // Create stock movement record
        var movement = new StockMovement
        {
            DrugId = batch.DrugId,
            BatchId = batch.Id,
            Type = MovementType.Adjustment,
            Quantity = request.QuantityChange,
            BalanceBefore = balanceBefore,
            BalanceAfter = newQuantity,
            Reference = $"ADJ-{DateTime.UtcNow:yyyyMMddHHmmss}",
            Reason = request.Reason,
            Notes = request.Notes
        };

        _context.StockMovements.Add(movement);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);

            var message = request.QuantityChange > 0
                ? $"Added {request.QuantityChange} units to {batch.Drug.Name}"
                : $"Removed {Math.Abs(request.QuantityChange)} units from {batch.Drug.Name}";

            return new AdjustStockResult(true, message, movement.Id);
        }
        catch (Exception ex)
        {
            return new AdjustStockResult(false, $"Error saving adjustment: {ex.Message}");
        }
    }
}
