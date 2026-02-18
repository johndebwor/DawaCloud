using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using DawaCloud.Web.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.Subscription.Commands;

public record UpdateSubscriptionCommand(
    int TenantId,
    string? StripeCustomerId,
    string? StripeSubscriptionId,
    int? SubscriptionPlanId,
    SubscriptionStatus Status,
    DateTime? CurrentPeriodEnd
) : IRequest<Result<bool>>;

public class UpdateSubscriptionCommandHandler : IRequestHandler<UpdateSubscriptionCommand, Result<bool>>
{
    private readonly AppDbContext _context;
    public UpdateSubscriptionCommandHandler(AppDbContext context) => _context = context;

    public async Task<Result<bool>> Handle(UpdateSubscriptionCommand request, CancellationToken ct)
    {
        var tenant = await _context.Tenants.FindAsync([request.TenantId], ct);
        if (tenant == null)
            return Result<bool>.Fail("Tenant not found");

        if (request.StripeCustomerId != null)
            tenant.StripeCustomerId = request.StripeCustomerId;
        if (request.StripeSubscriptionId != null)
            tenant.StripeSubscriptionId = request.StripeSubscriptionId;
        if (request.SubscriptionPlanId.HasValue)
        {
            tenant.SubscriptionPlanId = request.SubscriptionPlanId;
            var plan = await _context.SubscriptionPlans.FindAsync([request.SubscriptionPlanId.Value], ct);
            if (plan != null)
            {
                tenant.MaxUsers = plan.MaxUsers;
                tenant.MaxDrugs = plan.MaxDrugs;
                tenant.StorageLimitMb = plan.StorageLimitMb;
            }
        }
        tenant.SubscriptionStatus = request.Status;
        if (request.CurrentPeriodEnd.HasValue)
            tenant.CurrentPeriodEnd = request.CurrentPeriodEnd;
        tenant.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        return Result<bool>.Ok(true);
    }
}
