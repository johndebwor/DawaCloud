using DawaFlow.Web.Data;
using DawaFlow.Web.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaFlow.Web.Features.Inventory.Queries;

public class GetStockLevelsQueryHandler : IRequestHandler<GetStockLevelsQuery, List<StockLevelDto>>
{
    private readonly AppDbContext _context;

    public GetStockLevelsQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<StockLevelDto>> Handle(GetStockLevelsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Drugs
            .Include(d => d.Category)
            .Include(d => d.Batches.Where(b => b.Status == BatchStatus.Active))
            .AsQueryable();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(d =>
                d.Name.Contains(request.SearchTerm) ||
                d.Code.Contains(request.SearchTerm));
        }

        // Apply category filter
        if (!string.IsNullOrWhiteSpace(request.Category) && request.Category != "All")
        {
            query = query.Where(d => d.Category.Name == request.Category);
        }

        var drugs = await query
            .Where(d => d.IsActive)
            .OrderBy(d => d.Name)
            .ToListAsync(cancellationToken);

        // Map to DTOs with calculated values
        var stockLevels = drugs.Select(d =>
        {
            var availableQty = d.Batches.Sum(b => b.CurrentQuantity);
            var reservedQty = d.Batches.Sum(b => b.ReservedQuantity);
            var batchCount = d.Batches.Count;
            var totalValue = d.Batches.Sum(b => b.CurrentQuantity * b.CostPrice);

            // Determine stock level status
            var stockLevel = availableQty == 0 ? "Out" :
                           availableQty < d.ReorderLevel ? "Low" :
                           "Good";

            return new StockLevelDto
            {
                DrugId = d.Id,
                DrugCode = d.Code,
                DrugName = d.Name,
                Category = d.Category.Name,
                AvailableQuantity = availableQty,
                ReservedQuantity = reservedQty,
                ReorderLevel = d.ReorderLevel,
                Unit = GetUnit(d.DosageForm),
                BatchCount = batchCount,
                TotalValue = totalValue,
                StockLevel = stockLevel
            };
        }).ToList();

        // Apply stock level filter
        if (!string.IsNullOrWhiteSpace(request.StockLevel) && request.StockLevel != "All")
        {
            stockLevels = stockLevels.Where(s => s.StockLevel == request.StockLevel).ToList();
        }

        return stockLevels;
    }

    private static string GetUnit(string? dosageForm)
    {
        return dosageForm?.ToLower() switch
        {
            "tablet" or "tablets" => "Tablets",
            "capsule" or "capsules" => "Capsules",
            "syrup" or "suspension" => "ml",
            "injection" or "ampoule" => "Ampoules",
            "cream" or "ointment" => "g",
            _ => "Units"
        };
    }
}
