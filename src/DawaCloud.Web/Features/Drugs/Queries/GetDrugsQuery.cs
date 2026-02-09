using DawaCloud.Web.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.Drugs.Queries;

public record GetDrugsQuery(
    string? SearchTerm = null,
    int? CategoryId = null,
    string? Status = null,
    int Page = 1,
    int PageSize = 50
) : IRequest<GetDrugsResult>;

public record GetDrugsResult(
    bool Success,
    List<DrugListDto> Drugs,
    int TotalCount,
    string? ErrorMessage = null
);

public record DrugListDto(
    int Id,
    string Code,
    string Name,
    string? GenericName,
    string? Category,
    string? Manufacturer,
    int CurrentStock,
    int ReorderLevel,
    decimal RetailPrice,
    decimal WholesalePrice,
    decimal CostPrice,
    bool IsActive,
    bool RequiresPrescription
);

public class GetDrugsQueryHandler : IRequestHandler<GetDrugsQuery, GetDrugsResult>
{
    private readonly AppDbContext _context;
    private readonly ILogger<GetDrugsQueryHandler> _logger;

    public GetDrugsQueryHandler(AppDbContext context, ILogger<GetDrugsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<GetDrugsResult> Handle(GetDrugsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = _context.Drugs
                .Include(d => d.Category)
                .Include(d => d.Batches.Where(b => b.Status == Data.Entities.BatchStatus.Active))
                .AsNoTracking();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var search = request.SearchTerm.ToLower();
                query = query.Where(d =>
                    d.Name.ToLower().Contains(search) ||
                    d.Code.ToLower().Contains(search) ||
                    (d.Manufacturer != null && d.Manufacturer.ToLower().Contains(search)) ||
                    d.GenericName.ToLower().Contains(search));
            }

            // Apply category filter
            if (request.CategoryId.HasValue)
            {
                query = query.Where(d => d.CategoryId == request.CategoryId.Value);
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync(cancellationToken);

            // Get drugs with stock calculation
            var drugs = await query
                .OrderBy(d => d.Name)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(d => new DrugListDto(
                    d.Id,
                    d.Code,
                    d.Name,
                    d.GenericName,
                    d.Category != null ? d.Category.Name : null,
                    d.Manufacturer,
                    d.Batches.Sum(b => b.CurrentQuantity),
                    d.ReorderLevel,
                    d.RetailPrice,
                    d.WholesalePrice,
                    d.CostPrice,
                    d.IsActive,
                    d.RequiresPrescription
                ))
                .ToListAsync(cancellationToken);

            // Apply status filter (after stock calculation)
            if (!string.IsNullOrWhiteSpace(request.Status) && request.Status != "All")
            {
                drugs = request.Status switch
                {
                    "Active" or "In Stock" => drugs.Where(d => d.CurrentStock > d.ReorderLevel).ToList(),
                    "Low Stock" => drugs.Where(d => d.CurrentStock <= d.ReorderLevel && d.CurrentStock > 0).ToList(),
                    "Out of Stock" => drugs.Where(d => d.CurrentStock == 0).ToList(),
                    _ => drugs
                };
            }

            return new GetDrugsResult(true, drugs, totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching drugs");
            return new GetDrugsResult(false, new List<DrugListDto>(), 0, ex.Message);
        }
    }
}

public record GetDrugByIdQuery(int Id) : IRequest<GetDrugByIdResult>;

public record GetDrugByIdResult(
    bool Success,
    DrugDetailDto? Drug,
    string? ErrorMessage = null
);

public record DrugDetailDto(
    int Id,
    string Code,
    string? Barcode,
    string Name,
    string? GenericName,
    string? Description,
    int CategoryId,
    string? CategoryName,
    string? Manufacturer,
    string? DosageForm,
    string? Strength,
    string? PackSize,
    decimal RetailPrice,
    decimal WholesalePrice,
    decimal CostPrice,
    int ReorderLevel,
    int MaxStockLevel,
    decimal TaxRate,
    bool RequiresPrescription,
    bool IsControlled,
    bool IsActive,
    string? ImageUrl,
    int CurrentStock
);

public class GetDrugByIdQueryHandler : IRequestHandler<GetDrugByIdQuery, GetDrugByIdResult>
{
    private readonly AppDbContext _context;
    private readonly ILogger<GetDrugByIdQueryHandler> _logger;

    public GetDrugByIdQueryHandler(AppDbContext context, ILogger<GetDrugByIdQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<GetDrugByIdResult> Handle(GetDrugByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var drug = await _context.Drugs
                .Include(d => d.Category)
                .Include(d => d.Batches.Where(b => b.Status == Data.Entities.BatchStatus.Active))
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

            if (drug == null)
            {
                return new GetDrugByIdResult(false, null, "Drug not found");
            }

            var dto = new DrugDetailDto(
                drug.Id,
                drug.Code,
                drug.Barcode,
                drug.Name,
                drug.GenericName,
                drug.Description,
                drug.CategoryId,
                drug.Category?.Name,
                drug.Manufacturer,
                drug.DosageForm,
                drug.Strength,
                drug.PackSize,
                drug.RetailPrice,
                drug.WholesalePrice,
                drug.CostPrice,
                drug.ReorderLevel,
                drug.MaxStockLevel,
                drug.TaxRate,
                drug.RequiresPrescription,
                drug.IsControlled,
                drug.IsActive,
                drug.ImageUrl,
                drug.Batches.Sum(b => b.CurrentQuantity)
            );

            return new GetDrugByIdResult(true, dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching drug {Id}", request.Id);
            return new GetDrugByIdResult(false, null, ex.Message);
        }
    }
}

public record DeleteDrugCommand(int Id) : IRequest<DeleteDrugResult>;
public record DeleteDrugResult(bool Success, string Message);

public class DeleteDrugCommandHandler : IRequestHandler<DeleteDrugCommand, DeleteDrugResult>
{
    private readonly AppDbContext _context;
    private readonly ILogger<DeleteDrugCommandHandler> _logger;

    public DeleteDrugCommandHandler(AppDbContext context, ILogger<DeleteDrugCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<DeleteDrugResult> Handle(DeleteDrugCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var drug = await _context.Drugs
                .Include(d => d.Batches)
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

            if (drug == null)
            {
                return new DeleteDrugResult(false, "Drug not found");
            }

            // Check if there are any active batches with stock
            if (drug.Batches.Any(b => b.CurrentQuantity > 0))
            {
                return new DeleteDrugResult(false, "Cannot delete drug with existing stock. Please adjust stock first.");
            }

            // Soft delete
            drug.IsActive = false;
            drug.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Drug soft-deleted: {Id} - {Name}", drug.Id, drug.Name);
            return new DeleteDrugResult(true, "Drug deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting drug {Id}", request.Id);
            return new DeleteDrugResult(false, $"Error deleting drug: {ex.Message}");
        }
    }
}
