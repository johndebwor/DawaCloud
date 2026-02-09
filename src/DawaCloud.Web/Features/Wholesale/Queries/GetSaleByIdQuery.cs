using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.Wholesale.Queries;

public record GetSaleByIdQuery(int SaleId) : IRequest<SaleDto?>;

public class GetSaleByIdQueryHandler : IRequestHandler<GetSaleByIdQuery, SaleDto?>
{
    private readonly AppDbContext _context;

    public GetSaleByIdQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SaleDto?> Handle(GetSaleByIdQuery request, CancellationToken ct)
    {
        var sale = await _context.WholesaleSales
            .AsNoTracking()
            .Include(s => s.Customer)
            .Include(s => s.Items)
            .Where(s => s.Id == request.SaleId && !s.IsDeleted)
            .Select(s => new SaleDto(
                s.Id,
                s.InvoiceNumber,
                s.SaleDate,
                s.CustomerId,
                s.Customer.Name,
                s.SubTotal,
                s.TaxAmount,
                s.DiscountAmount,
                s.TotalAmount,
                s.PaidAmount,
                s.BalanceAmount,
                s.PaymentStatus,
                s.Status,
                s.Items.Count
            ))
            .FirstOrDefaultAsync(ct);

        return sale;
    }
}
