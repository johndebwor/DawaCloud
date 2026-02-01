using DawaFlow.Web.Data;
using DawaFlow.Web.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaFlow.Web.Features.Batches.Queries;

public record GetBatchesQuery(
    string? SearchTerm = null,
    string? Status = null,
    int Page = 1,
    int PageSize = 50
) : IRequest<GetBatchesResult>;

public record GetBatchesResult(
    bool Success,
    List<BatchListDto> Batches,
    int TotalCount,
    string? ErrorMessage = null
);

public record BatchListDto(
    int Id,
    string BatchNumber,
    int DrugId,
    string DrugName,
    DateTime ManufactureDate,
    DateTime ExpiryDate,
    int InitialQuantity,
    int CurrentQuantity,
    int ReservedQuantity,
    decimal CostPrice,
    string Status,
    int DaysToExpiry
);

public class GetBatchesQueryHandler : IRequestHandler<GetBatchesQuery, GetBatchesResult>
{
    private readonly AppDbContext _context;
    private readonly ILogger<GetBatchesQueryHandler> _logger;

    public GetBatchesQueryHandler(AppDbContext context, ILogger<GetBatchesQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<GetBatchesResult> Handle(GetBatchesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = _context.Batches
                .Include(b => b.Drug)
                .AsNoTracking();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var search = request.SearchTerm.ToLower();
                query = query.Where(b =>
                    b.BatchNumber.ToLower().Contains(search) ||
                    b.Drug.Name.ToLower().Contains(search));
            }

            // Apply status filter
            if (!string.IsNullOrWhiteSpace(request.Status) && request.Status != "All")
            {
                if (request.Status == "Expiring Soon")
                {
                    var expiryThreshold = DateTime.UtcNow.AddDays(30);
                    query = query.Where(b => b.ExpiryDate <= expiryThreshold && b.ExpiryDate > DateTime.UtcNow);
                }
                else if (Enum.TryParse<BatchStatus>(request.Status, out var batchStatus))
                {
                    query = query.Where(b => b.Status == batchStatus);
                }
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var batches = await query
                .OrderByDescending(b => b.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(b => new BatchListDto(
                    b.Id,
                    b.BatchNumber,
                    b.DrugId,
                    b.Drug.Name,
                    b.ManufactureDate,
                    b.ExpiryDate,
                    b.InitialQuantity,
                    b.CurrentQuantity,
                    b.ReservedQuantity,
                    b.CostPrice,
                    b.Status.ToString(),
                    (int)(b.ExpiryDate - DateTime.UtcNow).TotalDays
                ))
                .ToListAsync(cancellationToken);

            return new GetBatchesResult(true, batches, totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching batches");
            return new GetBatchesResult(false, new List<BatchListDto>(), 0, ex.Message);
        }
    }
}

public record GetBatchByIdQuery(int Id) : IRequest<GetBatchByIdResult>;

public record GetBatchByIdResult(
    bool Success,
    BatchDetailDto? Batch,
    string? ErrorMessage = null
);

public record BatchDetailDto(
    int Id,
    string BatchNumber,
    int DrugId,
    string DrugName,
    DateTime ManufactureDate,
    DateTime ExpiryDate,
    int InitialQuantity,
    int CurrentQuantity,
    int ReservedQuantity,
    decimal CostPrice,
    string? SupplierBatchRef,
    int? GoodsReceiptId,
    BatchStatus Status
);

public class GetBatchByIdQueryHandler : IRequestHandler<GetBatchByIdQuery, GetBatchByIdResult>
{
    private readonly AppDbContext _context;
    private readonly ILogger<GetBatchByIdQueryHandler> _logger;

    public GetBatchByIdQueryHandler(AppDbContext context, ILogger<GetBatchByIdQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<GetBatchByIdResult> Handle(GetBatchByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var batch = await _context.Batches
                .Include(b => b.Drug)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (batch == null)
            {
                return new GetBatchByIdResult(false, null, "Batch not found");
            }

            var dto = new BatchDetailDto(
                batch.Id,
                batch.BatchNumber,
                batch.DrugId,
                batch.Drug.Name,
                batch.ManufactureDate,
                batch.ExpiryDate,
                batch.InitialQuantity,
                batch.CurrentQuantity,
                batch.ReservedQuantity,
                batch.CostPrice,
                batch.SupplierBatchRef,
                batch.GoodsReceiptId,
                batch.Status
            );

            return new GetBatchByIdResult(true, dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching batch {Id}", request.Id);
            return new GetBatchByIdResult(false, null, ex.Message);
        }
    }
}
