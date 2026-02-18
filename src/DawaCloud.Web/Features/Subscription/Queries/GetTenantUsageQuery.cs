using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using DawaCloud.Web.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.Subscription.Queries;

public record GetTenantUsageQuery(int TenantId) : IRequest<TenantUsageDto?>;

public record TenantUsageDto(
    string TenantName,
    string PlanName,
    decimal PriceMonthly,
    SubscriptionStatus Status,
    DateTime? TrialEndsAt,
    DateTime? CurrentPeriodEnd,
    int CurrentUsers,
    int MaxUsers,
    int CurrentDrugs,
    int MaxDrugs,
    decimal StorageUsedMb,
    int StorageLimitMb
);

public class GetTenantUsageQueryHandler : IRequestHandler<GetTenantUsageQuery, TenantUsageDto?>
{
    private readonly AppDbContext _context;

    public GetTenantUsageQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TenantUsageDto?> Handle(GetTenantUsageQuery request, CancellationToken ct)
    {
        var tenant = await _context.Tenants
            .AsNoTracking()
            .Include(t => t.SubscriptionPlan)
            .FirstOrDefaultAsync(t => t.Id == request.TenantId, ct);

        if (tenant == null) return null;

        var currentDrugs = await _context.Drugs.CountAsync(ct);

        return new TenantUsageDto(
            tenant.Name,
            tenant.SubscriptionPlan?.Name ?? "Free Trial",
            tenant.SubscriptionPlan?.PriceMonthly ?? 0,
            tenant.SubscriptionStatus,
            tenant.TrialEndsAt,
            tenant.CurrentPeriodEnd,
            0, // Will be calculated from user manager
            tenant.MaxUsers,
            currentDrugs,
            tenant.MaxDrugs,
            tenant.StorageUsedMb,
            tenant.StorageLimitMb
        );
    }
}
