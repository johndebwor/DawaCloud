using DawaFlow.Web.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaFlow.Web.Features.Dashboard.Queries;

public record GetSalesTrendQuery(int Days) : IRequest<SalesTrendResult>;

public record SalesTrendResult(
    string[] Labels,
    double[] RetailTotals,
    double[] WholesaleTotals,
    decimal TotalAmount,
    decimal DailyAverage,
    decimal BestDay
);

public class GetSalesTrendQueryHandler : IRequestHandler<GetSalesTrendQuery, SalesTrendResult>
{
    private readonly AppDbContext _context;

    public GetSalesTrendQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SalesTrendResult> Handle(GetSalesTrendQuery request, CancellationToken ct)
    {
        var startDate = DateTime.UtcNow.Date.AddDays(-request.Days + 1);
        var endDate = DateTime.UtcNow.Date;

        var retailByDay = await _context.RetailSales
            .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate.AddDays(1))
            .GroupBy(s => s.SaleDate.Date)
            .Select(g => new { Date = g.Key, Total = g.Sum(s => s.TotalAmount) })
            .ToDictionaryAsync(x => x.Date, x => x.Total, ct);

        var wholesaleByDay = await _context.WholesaleSales
            .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate.AddDays(1))
            .GroupBy(s => s.SaleDate.Date)
            .Select(g => new { Date = g.Key, Total = g.Sum(s => s.TotalAmount) })
            .ToDictionaryAsync(x => x.Date, x => x.Total, ct);

        var labels = new List<string>();
        var retailTotals = new List<double>();
        var wholesaleTotals = new List<double>();

        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            labels.Add(request.Days <= 7 ? date.ToString("ddd") : date.ToString("MMM d"));
            retailTotals.Add((double)(retailByDay.TryGetValue(date, out var r) ? r : 0m));
            wholesaleTotals.Add((double)(wholesaleByDay.TryGetValue(date, out var w) ? w : 0m));
        }

        var combinedTotals = labels.Select((_, i) => (decimal)(retailTotals[i] + wholesaleTotals[i])).ToList();
        var totalAmount = combinedTotals.Sum();
        var dailyAverage = labels.Count > 0 ? totalAmount / labels.Count : 0;
        var bestDay = combinedTotals.Any() ? combinedTotals.Max() : 0;

        return new SalesTrendResult(
            labels.ToArray(),
            retailTotals.ToArray(),
            wholesaleTotals.ToArray(),
            totalAmount,
            dailyAverage,
            bestDay
        );
    }
}
