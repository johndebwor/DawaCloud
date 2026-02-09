using DawaCloud.Web.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.Retail.Queries;

public record GetHeldSalesQuery(string? CashierId = null) : IRequest<List<HeldSaleListDto>>;

public record HeldSaleListDto(
    int Id,
    string Reference,
    string? CustomerName,
    decimal TotalAmount,
    DateTime HeldAt
);

public class GetHeldSalesQueryHandler : IRequestHandler<GetHeldSalesQuery, List<HeldSaleListDto>>
{
    private readonly AppDbContext _context;

    public GetHeldSalesQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<HeldSaleListDto>> Handle(GetHeldSalesQuery request, CancellationToken ct)
    {
        var query = _context.HeldSales
            .AsNoTracking()
            .Where(h => !h.IsRecalled);

        if (!string.IsNullOrEmpty(request.CashierId))
            query = query.Where(h => h.CashierId == request.CashierId);

        var heldSales = await query
            .OrderByDescending(h => h.HeldAt)
            .Select(h => new HeldSaleListDto(
                h.Id,
                h.Reference,
                h.CustomerName,
                h.TotalAmount,
                h.HeldAt
            ))
            .ToListAsync(ct);

        return heldSales;
    }
}
