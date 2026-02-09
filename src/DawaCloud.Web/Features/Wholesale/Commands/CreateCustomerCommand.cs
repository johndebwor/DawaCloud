using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.Wholesale.Commands;

public record CreateCustomerCommand(
    string Name,
    string? TaxId,
    string? Email,
    string? Phone,
    string? Address,
    string? City,
    decimal CreditLimit,
    string PricingTier
) : IRequest<Result<int>>;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Result<int>>
{
    private readonly AppDbContext _context;

    public CreateCustomerCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(CreateCustomerCommand request, CancellationToken ct)
    {
        // Generate unique customer code
        var lastCustomer = await _context.WholesaleCustomers
            .OrderByDescending(c => c.Id)
            .FirstOrDefaultAsync(ct);

        var nextNumber = (lastCustomer?.Id ?? 0) + 1;
        var customerCode = $"CUST-{nextNumber:D5}";

        // Parse PricingTier from string
        if (!Enum.TryParse<PricingTier>(request.PricingTier, out var pricingTier))
        {
            pricingTier = PricingTier.Standard; // Default value
        }

        var customer = new WholesaleCustomer
        {
            Code = customerCode,
            Name = request.Name,
            TaxId = request.TaxId,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
            City = request.City,
            CreditLimit = request.CreditLimit,
            CurrentBalance = 0,
            PricingTier = pricingTier,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.WholesaleCustomers.Add(customer);
        await _context.SaveChangesAsync(ct);

        return Result<int>.Ok(customer.Id);
    }
}
