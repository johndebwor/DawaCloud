using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Infrastructure.Middleware;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly HashSet<string> ExemptPrefixes = new(StringComparer.OrdinalIgnoreCase)
    {
        "/welcome",
        "/auth/",
        "/api/auth",
        "/api/stripe",
        "/subscription/",
        "/_blazor",
        "/_framework",
        "/css",
        "/js",
        "/images",
        "/_content",
        "/favicon"
    };

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
    {
        var path = context.Request.Path.Value ?? "";

        // Skip exempt paths
        if (ExemptPrefixes.Any(prefix => path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            || path == "/")
        {
            await _next(context);
            return;
        }

        if (context.User.Identity?.IsAuthenticated == true)
        {
            var tenantIdClaim = context.User.FindFirst("TenantId")?.Value;
            if (int.TryParse(tenantIdClaim, out var tenantId))
            {
                var tenant = await dbContext.Tenants
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == tenantId);

                if (tenant != null)
                {
                    var isExpired = tenant.SubscriptionStatus switch
                    {
                        SubscriptionStatus.Cancelled => true,
                        SubscriptionStatus.Expired => true,
                        SubscriptionStatus.Trialing when tenant.TrialEndsAt.HasValue
                            && tenant.TrialEndsAt.Value < DateTime.UtcNow => true,
                        _ => false
                    };

                    if (isExpired && !path.StartsWith("/subscription", StringComparison.OrdinalIgnoreCase))
                    {
                        context.Response.Redirect("/subscription/expired");
                        return;
                    }
                }
            }
        }

        await _next(context);
    }
}
