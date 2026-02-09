using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.Wholesale.Commands;

public record UpdateCustomerCommand(
    int Id,
    string Name,
    string? TaxId,
    string? Email,
    string? Phone,
    string? Address,
    string? City,
    decimal CreditLimit,
    string PricingTier,
    bool IsActive
) : IRequest<Result>;

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Result>
{
    private readonly AppDbContext _context;

    public UpdateCustomerCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateCustomerCommand request, CancellationToken ct)
    {
        var customer = await _context.WholesaleCustomers
            .FirstOrDefaultAsync(c => c.Id == request.Id, ct);

        if (customer == null)
            return Result.Fail("Customer not found");

        // Parse PricingTier from string
        if (!Enum.TryParse<PricingTier>(request.PricingTier, out var pricingTier))
        {
            pricingTier = PricingTier.Standard; // Default value
        }

        customer.Name = request.Name;
        customer.TaxId = request.TaxId;
        customer.Email = request.Email;
        customer.Phone = request.Phone;
        customer.Address = request.Address;
        customer.City = request.City;
        customer.CreditLimit = request.CreditLimit;
        customer.PricingTier = pricingTier;
        customer.IsActive = request.IsActive;
        customer.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        return Result.Ok();
    }
}
