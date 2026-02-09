using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.Wholesale.Queries;

public record GetQuotationsQuery(
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    int? CustomerId = null,
    QuotationStatus? Status = null,
    string? SearchTerm = null
) : IRequest<List<QuotationDto>>;

public record QuotationDto(
    int Id,
    string QuotationNumber,
    DateTime QuotationDate,
    DateTime ValidUntil,
    int CustomerId,
    string CustomerName,
    decimal SubTotal,
    decimal TaxAmount,
    decimal DiscountAmount,
    decimal TotalAmount,
    QuotationStatus Status,
    int ItemCount,
    bool IsExpired,
    int? ConvertedToSaleId
);

public class GetQuotationsQueryHandler : IRequestHandler<GetQuotationsQuery, List<QuotationDto>>
{
    private readonly AppDbContext _context;

    public GetQuotationsQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<QuotationDto>> Handle(GetQuotationsQuery request, CancellationToken ct)
    {
        var query = _context.Quotations
            .AsNoTracking()
            .Include(q => q.Customer)
            .Include(q => q.Items)
            .Where(q => !q.IsDeleted);

        if (request.StartDate.HasValue)
            query = query.Where(q => q.QuotationDate >= request.StartDate.Value);

        if (request.EndDate.HasValue)
            query = query.Where(q => q.QuotationDate <= request.EndDate.Value);

        if (request.CustomerId.HasValue)
            query = query.Where(q => q.CustomerId == request.CustomerId.Value);

        if (request.Status.HasValue)
            query = query.Where(q => q.Status == request.Status.Value);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(q =>
                q.QuotationNumber.Contains(request.SearchTerm) ||
                q.Customer.Name.Contains(request.SearchTerm)
            );
        }

        var quotations = await query
            .OrderByDescending(q => q.QuotationDate)
            .Select(q => new QuotationDto(
                q.Id,
                q.QuotationNumber,
                q.QuotationDate,
                q.ValidUntil,
                q.CustomerId,
                q.Customer.Name,
                q.SubTotal,
                q.TaxAmount,
                q.DiscountAmount,
                q.TotalAmount,
                q.Status,
                q.Items.Count,
                q.ValidUntil < DateTime.Now,
                q.ConvertedToSaleId
            ))
            .ToListAsync(ct);

        return quotations;
    }
}
