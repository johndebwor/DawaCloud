using DawaFlow.Web.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaFlow.Web.Features.Inventory.Queries;

public record GetStockCountDetailsQuery(int StockCountId) : IRequest<StockCountDetailsDto?>;

public class StockCountDetailsDto
{
    public int Id { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public DateTime CountDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public int TotalItems { get; set; }
    public int TotalItemsCounted { get; set; }
    public int ItemsWithVariance { get; set; }
    public decimal TotalVarianceValue { get; set; }
    public string? CompletedBy { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<StockCountItemDto> Items { get; set; } = new();
}

public class StockCountItemDto
{
    public int Id { get; set; }
    public int DrugId { get; set; }
    public string DrugName { get; set; } = string.Empty;
    public string DrugCode { get; set; } = string.Empty;
    public int BatchId { get; set; }
    public string BatchNumber { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public int SystemQuantity { get; set; }
    public int PhysicalQuantity { get; set; }
    public int Variance { get; set; }
    public decimal VarianceValue { get; set; }
    public string? CountedBy { get; set; }
    public DateTime? CountedAt { get; set; }
    public string? Notes { get; set; }
    public bool IsCounted => CountedAt.HasValue;
}

public class GetStockCountDetailsQueryHandler : IRequestHandler<GetStockCountDetailsQuery, StockCountDetailsDto?>
{
    private readonly AppDbContext _context;

    public GetStockCountDetailsQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<StockCountDetailsDto?> Handle(GetStockCountDetailsQuery request, CancellationToken cancellationToken)
    {
        var stockCount = await _context.StockCounts
            .Include(sc => sc.Items)
            .ThenInclude(sci => sci.Drug)
            .Include(sc => sc.Items)
            .ThenInclude(sci => sci.Batch)
            .FirstOrDefaultAsync(sc => sc.Id == request.StockCountId, cancellationToken);

        if (stockCount == null)
        {
            return null;
        }

        return new StockCountDetailsDto
        {
            Id = stockCount.Id,
            ReferenceNumber = stockCount.ReferenceNumber,
            CountDate = stockCount.CountDate,
            Status = stockCount.Status.ToString(),
            Notes = stockCount.Notes,
            TotalItems = stockCount.Items.Count,
            TotalItemsCounted = stockCount.TotalItemsCounted,
            ItemsWithVariance = stockCount.ItemsWithVariance,
            TotalVarianceValue = stockCount.TotalVarianceValue,
            CompletedBy = stockCount.CompletedBy,
            CompletedAt = stockCount.CompletedAt,
            Items = stockCount.Items.Select(sci => new StockCountItemDto
            {
                Id = sci.Id,
                DrugId = sci.DrugId,
                DrugName = sci.Drug.Name,
                DrugCode = sci.Drug.Code,
                BatchId = sci.BatchId,
                BatchNumber = sci.Batch.BatchNumber,
                ExpiryDate = sci.Batch.ExpiryDate,
                SystemQuantity = sci.SystemQuantity,
                PhysicalQuantity = sci.PhysicalQuantity,
                Variance = sci.Variance,
                VarianceValue = sci.VarianceValue,
                CountedBy = sci.CountedBy,
                CountedAt = sci.CountedAt,
                Notes = sci.Notes
            }).ToList()
        };
    }
}
