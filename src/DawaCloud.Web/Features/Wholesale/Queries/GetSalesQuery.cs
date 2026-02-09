using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.Wholesale.Queries;

public record GetSalesQuery(
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    int? CustomerId = null,
    PaymentStatus? PaymentStatus = null,
    string? SearchTerm = null
) : IRequest<List<SaleDto>>;

public record SaleDto(
    int Id,
    string InvoiceNumber,
    DateTime SaleDate,
    int CustomerId,
    string CustomerName,
    decimal SubTotal,
    decimal TaxAmount,
    decimal DiscountAmount,
    decimal TotalAmount,
    decimal PaidAmount,
    decimal BalanceAmount,
    PaymentStatus PaymentStatus,
    SaleStatus Status,
    int ItemCount
);

public class GetSalesQueryHandler : IRequestHandler<GetSalesQuery, List<SaleDto>>
{
    private readonly AppDbContext _context;

    public GetSalesQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SaleDto>> Handle(GetSalesQuery request, CancellationToken ct)
    {
        var query = _context.WholesaleSales
            .AsNoTracking()
            .Include(s => s.Customer)
            .Include(s => s.Items)
            .Where(s => !s.IsDeleted);

        if (request.StartDate.HasValue)
            query = query.Where(s => s.SaleDate >= request.StartDate.Value);

        if (request.EndDate.HasValue)
            query = query.Where(s => s.SaleDate <= request.EndDate.Value);

        if (request.CustomerId.HasValue)
            query = query.Where(s => s.CustomerId == request.CustomerId.Value);

        if (request.PaymentStatus.HasValue)
            query = query.Where(s => s.PaymentStatus == request.PaymentStatus.Value);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(s =>
                s.InvoiceNumber.Contains(request.SearchTerm) ||
                s.Customer.Name.Contains(request.SearchTerm)
            );
        }

        var sales = await query
            .OrderByDescending(s => s.SaleDate)
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
            .ToListAsync(ct);

        return sales;
    }
}
