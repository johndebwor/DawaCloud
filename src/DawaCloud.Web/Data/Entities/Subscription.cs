namespace DawaCloud.Web.Data.Entities;

public class Tenant : BaseAuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Subdomain { get; set; }

    // Stripe Integration
    public string? StripeCustomerId { get; set; }
    public string? StripeSubscriptionId { get; set; }

    // Subscription
    public int? SubscriptionPlanId { get; set; }
    public SubscriptionStatus SubscriptionStatus { get; set; } = SubscriptionStatus.Trialing;
    public DateTime? TrialEndsAt { get; set; }
    public DateTime? CurrentPeriodEnd { get; set; }

    // Limits
    public int MaxUsers { get; set; } = 2;
    public int MaxDrugs { get; set; } = 100;
    public int StorageLimitMb { get; set; } = 500;
    public decimal StorageUsedMb { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation
    public SubscriptionPlan? SubscriptionPlan { get; set; }
}

public class SubscriptionPlan : BaseAuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? StripePriceIdMonthly { get; set; }
    public string? StripePriceIdAnnual { get; set; }
    public decimal PriceMonthly { get; set; }
    public decimal PriceAnnual { get; set; }
    public string? FeaturesJson { get; set; }
    public int MaxUsers { get; set; }
    public int MaxDrugs { get; set; }
    public int StorageLimitMb { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
    public bool IsFeatured { get; set; }
}

public enum SubscriptionStatus
{
    Trialing,
    Active,
    PastDue,
    Cancelled,
    Expired,
    Incomplete
}
