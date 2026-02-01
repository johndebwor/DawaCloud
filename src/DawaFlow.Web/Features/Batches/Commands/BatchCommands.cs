using DawaFlow.Web.Data;
using DawaFlow.Web.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaFlow.Web.Features.Batches.Commands;

public record CreateBatchCommand(
    string BatchNumber,
    int DrugId,
    DateTime ManufactureDate,
    DateTime ExpiryDate,
    int InitialQuantity,
    decimal CostPrice,
    string? SupplierBatchRef
) : IRequest<BatchCommandResult>;

public record UpdateBatchCommand(
    int Id,
    string BatchNumber,
    int DrugId,
    DateTime ManufactureDate,
    DateTime ExpiryDate,
    int InitialQuantity,
    int CurrentQuantity,
    decimal CostPrice,
    string? SupplierBatchRef,
    BatchStatus Status
) : IRequest<BatchCommandResult>;

public record QuarantineBatchCommand(int Id, string? Reason = null) : IRequest<BatchCommandResult>;

public record BatchCommandResult(bool Success, string Message, int? BatchId = null);

public class CreateBatchCommandHandler : IRequestHandler<CreateBatchCommand, BatchCommandResult>
{
    private readonly AppDbContext _context;
    private readonly ILogger<CreateBatchCommandHandler> _logger;

    public CreateBatchCommandHandler(AppDbContext context, ILogger<CreateBatchCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<BatchCommandResult> Handle(CreateBatchCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if batch number already exists
            var exists = await _context.Batches
                .AnyAsync(b => b.BatchNumber == request.BatchNumber, cancellationToken);

            if (exists)
            {
                return new BatchCommandResult(false, "A batch with this number already exists");
            }

            // Verify drug exists
            var drugExists = await _context.Drugs
                .AnyAsync(d => d.Id == request.DrugId, cancellationToken);

            if (!drugExists)
            {
                return new BatchCommandResult(false, "The selected drug does not exist");
            }

            var batch = new Batch
            {
                BatchNumber = request.BatchNumber,
                DrugId = request.DrugId,
                ManufactureDate = request.ManufactureDate,
                ExpiryDate = request.ExpiryDate,
                InitialQuantity = request.InitialQuantity,
                CurrentQuantity = request.InitialQuantity,
                ReservedQuantity = 0,
                CostPrice = request.CostPrice,
                SupplierBatchRef = request.SupplierBatchRef,
                Status = BatchStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            _context.Batches.Add(batch);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Batch created: {BatchNumber} for Drug {DrugId}", batch.BatchNumber, batch.DrugId);
            return new BatchCommandResult(true, "Batch created successfully", batch.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating batch");
            return new BatchCommandResult(false, $"Error creating batch: {ex.Message}");
        }
    }
}

public class UpdateBatchCommandHandler : IRequestHandler<UpdateBatchCommand, BatchCommandResult>
{
    private readonly AppDbContext _context;
    private readonly ILogger<UpdateBatchCommandHandler> _logger;

    public UpdateBatchCommandHandler(AppDbContext context, ILogger<UpdateBatchCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<BatchCommandResult> Handle(UpdateBatchCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var batch = await _context.Batches
                .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (batch == null)
            {
                return new BatchCommandResult(false, "Batch not found");
            }

            // Check if batch number is being changed and if new number already exists
            if (batch.BatchNumber != request.BatchNumber)
            {
                var exists = await _context.Batches
                    .AnyAsync(b => b.BatchNumber == request.BatchNumber && b.Id != request.Id, cancellationToken);

                if (exists)
                {
                    return new BatchCommandResult(false, "A batch with this number already exists");
                }
            }

            // Validate current quantity
            if (request.CurrentQuantity > request.InitialQuantity)
            {
                return new BatchCommandResult(false, "Current quantity cannot exceed initial quantity");
            }

            batch.BatchNumber = request.BatchNumber;
            batch.DrugId = request.DrugId;
            batch.ManufactureDate = request.ManufactureDate;
            batch.ExpiryDate = request.ExpiryDate;
            batch.InitialQuantity = request.InitialQuantity;
            batch.CurrentQuantity = request.CurrentQuantity;
            batch.CostPrice = request.CostPrice;
            batch.SupplierBatchRef = request.SupplierBatchRef;
            batch.Status = request.Status;
            batch.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Batch updated: {BatchNumber}", batch.BatchNumber);
            return new BatchCommandResult(true, "Batch updated successfully", batch.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating batch {Id}", request.Id);
            return new BatchCommandResult(false, $"Error updating batch: {ex.Message}");
        }
    }
}

public class QuarantineBatchCommandHandler : IRequestHandler<QuarantineBatchCommand, BatchCommandResult>
{
    private readonly AppDbContext _context;
    private readonly ILogger<QuarantineBatchCommandHandler> _logger;

    public QuarantineBatchCommandHandler(AppDbContext context, ILogger<QuarantineBatchCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<BatchCommandResult> Handle(QuarantineBatchCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var batch = await _context.Batches
                .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (batch == null)
            {
                return new BatchCommandResult(false, "Batch not found");
            }

            if (batch.Status == BatchStatus.Quarantined)
            {
                return new BatchCommandResult(false, "Batch is already quarantined");
            }

            batch.Status = BatchStatus.Quarantined;
            batch.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogWarning("Batch quarantined: {BatchNumber}. Reason: {Reason}",
                batch.BatchNumber, request.Reason ?? "Not specified");

            return new BatchCommandResult(true, $"Batch {batch.BatchNumber} has been quarantined", batch.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error quarantining batch {Id}", request.Id);
            return new BatchCommandResult(false, $"Error quarantining batch: {ex.Message}");
        }
    }
}
