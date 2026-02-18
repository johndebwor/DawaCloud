using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using DawaCloud.Web.Shared;
using MediatR;

namespace DawaCloud.Web.Features.Subscription.Commands;

public record CreateTenantCommand(
    string Name,
    string? Subdomain
) : IRequest<Result<int>>;

public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, Result<int>>
{
    private readonly AppDbContext _context;
    public CreateTenantCommandHandler(AppDbContext context) => _context = context;

    public async Task<Result<int>> Handle(CreateTenantCommand request, CancellationToken ct)
    {
        var tenant = new Tenant
        {
            Name = request.Name,
            Subdomain = request.Subdomain,
            SubscriptionStatus = SubscriptionStatus.Trialing,
            TrialEndsAt = DateTime.UtcNow.AddDays(14),
            MaxUsers = 2,
            MaxDrugs = 100,
            StorageLimitMb = 500,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync(ct);

        return Result<int>.Ok(tenant.Id);
    }
}
