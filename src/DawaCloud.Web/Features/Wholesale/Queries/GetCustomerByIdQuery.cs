using DawaCloud.Web.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.Wholesale.Queries;

public record GetCustomerByIdQuery(int Id) : IRequest<CustomerDto?>;

public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDto?>
{
    private readonly AppDbContext _context;

    public GetCustomerByIdQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CustomerDto?> Handle(GetCustomerByIdQuery request, CancellationToken ct)
    {
        var customer = await _context.WholesaleCustomers
            .AsNoTracking()
            .Where(c => c.Id == request.Id && !c.IsDeleted)
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
            .FirstOrDefaultAsync(ct);

        return customer;
    }
}
