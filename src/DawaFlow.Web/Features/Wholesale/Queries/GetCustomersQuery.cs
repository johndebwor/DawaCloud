using DawaFlow.Web.Data;
using DawaFlow.Web.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaFlow.Web.Features.Wholesale.Queries;

public record GetCustomersQuery(
    string? SearchTerm = null,
    string? PricingTier = null,
    bool ActiveOnly = true
) : IRequest<List<CustomerDto>>;

public record CustomerDto(
    int Id,
    string Code,
    string Name,
    string? TaxId,
    string? Email,
    string? Phone,
    string? Address,
    string? City,
    decimal CreditLimit,
    decimal CurrentBalance,
    string PricingTier,
    bool IsActive
);

public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, List<CustomerDto>>
{
    private readonly AppDbContext _context;

    public GetCustomersQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<CustomerDto>> Handle(GetCustomersQuery request, CancellationToken ct)
    {
        var query = _context.WholesaleCustomers
            .AsNoTracking()
            .Where(c => !c.IsDeleted);

        if (request.ActiveOnly)
            query = query.Where(c => c.IsActive);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(c =>
                c.Name.Contains(request.SearchTerm) ||
                c.Code.Contains(request.SearchTerm) ||
                (c.Email != null && c.Email.Contains(request.SearchTerm)) ||
                (c.Phone != null && c.Phone.Contains(request.SearchTerm))
            );
        }

        if (!string.IsNullOrWhiteSpace(request.PricingTier) && request.PricingTier != "All")
        {
            if (Enum.TryParse<PricingTier>(request.PricingTier, out var pricingTierFilter))
            {
                query = query.Where(c => c.PricingTier == pricingTierFilter);
            }
        }

        var customers = await query
            .OrderBy(c => c.Name)
            .Select(c => new CustomerDto(
                c.Id,
                c.Code,
                c.Name,
                c.TaxId,
                c.Email,
                c.Phone,
                c.Address,
                c.City,
                c.CreditLimit,
                c.CurrentBalance,
                c.PricingTier.ToString(),
                c.IsActive
            ))
            .ToListAsync(ct);

        return customers;
    }
}
