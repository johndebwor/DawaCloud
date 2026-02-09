using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.Inventory.Queries;

public record GetStockCountsQuery(
    StockCountStatus? Status = null
) : IRequest<List<StockCountSummaryDto>>;

public class StockCountSummaryDto
{
    public int Id { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public DateTime CountDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public int TotalItems { get; set; }
    public int TotalItemsCounted { get; set; }
    public int ItemsWithVariance { get; set; }
    public decimal TotalVarianceValue { get; set; }
    public string? CompletedBy { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}

public class GetStockCountsQueryHandler : IRequestHandler<GetStockCountsQuery, List<StockCountSummaryDto>>
{
    private readonly AppDbContext _context;

    public GetStockCountsQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<StockCountSummaryDto>> Handle(GetStockCountsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.StockCounts.AsQueryable();

        if (request.Status.HasValue)
        {
            query = query.Where(sc => sc.Status == request.Status.Value);
        }

        var stockCounts = await query
            .OrderByDescending(sc => sc.CountDate)
            .Select(sc => new StockCountSummaryDto
            {
                Id = sc.Id,
                ReferenceNumber = sc.ReferenceNumber,
                CountDate = sc.CountDate,
                Status = sc.Status.ToString(),
                TotalItems = sc.Items.Count,
                TotalItemsCounted = sc.TotalItemsCounted,
                ItemsWithVariance = sc.ItemsWithVariance,
                TotalVarianceValue = sc.TotalVarianceValue,
                CompletedBy = sc.CompletedBy,
                CompletedAt = sc.CompletedAt,
                CreatedBy = sc.CreatedBy ?? "System"
            })
            .ToListAsync(cancellationToken);

        return stockCounts;
    }
}
