using DawaCloud.Web.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.Subscription.Queries;

public record GetSubscriptionPlansQuery : IRequest<List<SubscriptionPlanDto>>;

public record SubscriptionPlanDto(
    int Id,
    string Name,
    decimal PriceMonthly,
    decimal PriceAnnual,
    string? FeaturesJson,
    int MaxUsers,
    int MaxDrugs,
    int StorageLimitMb,
    bool IsFeatured,
    int SortOrder
);

public class GetSubscriptionPlansQueryHandler : IRequestHandler<GetSubscriptionPlansQuery, List<SubscriptionPlanDto>>
{
    private readonly AppDbContext _context;
    public GetSubscriptionPlansQueryHandler(AppDbContext context) => _context = context;

    public async Task<List<SubscriptionPlanDto>> Handle(GetSubscriptionPlansQuery request, CancellationToken ct)
    {
        return await _context.SubscriptionPlans
            .AsNoTracking()
            .Where(p => p.IsActive)
            .OrderBy(p => p.SortOrder)
            .Select(p => new SubscriptionPlanDto(
                p.Id,
                p.Name,
                p.PriceMonthly,
                p.PriceAnnual,
                p.FeaturesJson,
                p.MaxUsers,
                p.MaxDrugs,
                p.StorageLimitMb,
                p.IsFeatured,
                p.SortOrder
            ))
            .ToListAsync(ct);
    }
}
