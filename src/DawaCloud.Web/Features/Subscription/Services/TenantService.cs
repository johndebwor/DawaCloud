using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.Subscription.Services;

public interface ITenantService
{
    Task<Tenant?> GetTenantAsync(int tenantId);
    Task<bool> IsSubscriptionActiveAsync(int tenantId);
}

public class TenantService : ITenantService
{
    private readonly AppDbContext _context;

    public TenantService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Tenant?> GetTenantAsync(int tenantId)
    {
        return await _context.Tenants
            .AsNoTracking()
            .Include(t => t.SubscriptionPlan)
            .FirstOrDefaultAsync(t => t.Id == tenantId);
    }

    public async Task<bool> IsSubscriptionActiveAsync(int tenantId)
    {
        var tenant = await _context.Tenants.FindAsync(tenantId);
        if (tenant == null) return false;

        return tenant.SubscriptionStatus switch
        {
            SubscriptionStatus.Active => true,
            SubscriptionStatus.Trialing => tenant.TrialEndsAt.HasValue && tenant.TrialEndsAt.Value > DateTime.UtcNow,
            _ => false
        };
    }
}
