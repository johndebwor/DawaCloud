using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace DawaCloud.Web.Features.Subscription.Services;

public class StripeSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string PublishableKey { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;
}

public interface IStripeService
{
    Task<string> CreateCheckoutSessionAsync(int tenantId, string stripePriceId, string successUrl, string cancelUrl);
    Task<string> CreateCustomerPortalSessionAsync(int tenantId, string returnUrl);
    Task HandleWebhookAsync(string json, string stripeSignature);
    string GetPublishableKey();
}

public class StripeService : IStripeService
{
    private readonly AppDbContext _context;
    private readonly StripeSettings _settings;
    private readonly ILogger<StripeService> _logger;

    public StripeService(AppDbContext context, IOptions<StripeSettings> settings, ILogger<StripeService> logger)
    {
        _context = context;
        _settings = settings.Value;
        _logger = logger;
        StripeConfiguration.ApiKey = _settings.SecretKey;
    }

    public string GetPublishableKey() => _settings.PublishableKey;

    public async Task<string> CreateCheckoutSessionAsync(int tenantId, string stripePriceId, string successUrl, string cancelUrl)
    {
        var tenant = await _context.Tenants.FindAsync(tenantId);
        if (tenant == null) throw new InvalidOperationException("Tenant not found");

        var options = new SessionCreateOptions
        {
            Mode = "subscription",
            SuccessUrl = successUrl,
            CancelUrl = cancelUrl,
            LineItems = new List<SessionLineItemOptions>
            {
                new()
                {
                    Price = stripePriceId,
                    Quantity = 1
                }
            },
            Metadata = new Dictionary<string, string>
            {
                { "tenantId", tenantId.ToString() }
            }
        };

        if (!string.IsNullOrEmpty(tenant.StripeCustomerId))
        {
            options.Customer = tenant.StripeCustomerId;
        }
        else
        {
            options.CustomerEmail = tenant.Name; // Will be updated
        }

        var service = new SessionService();
        var session = await service.CreateAsync(options);

        return session.Url;
    }

    public async Task<string> CreateCustomerPortalSessionAsync(int tenantId, string returnUrl)
    {
        var tenant = await _context.Tenants.FindAsync(tenantId);
        if (tenant == null || string.IsNullOrEmpty(tenant.StripeCustomerId))
            throw new InvalidOperationException("Tenant has no Stripe customer");

        var options = new Stripe.BillingPortal.SessionCreateOptions
        {
            Customer = tenant.StripeCustomerId,
            ReturnUrl = returnUrl
        };

        var service = new Stripe.BillingPortal.SessionService();
        var session = await service.CreateAsync(options);

        return session.Url;
    }

    public async Task HandleWebhookAsync(string json, string stripeSignature)
    {
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, _settings.WebhookSecret);

            switch (stripeEvent.Type)
            {
                case EventTypes.CheckoutSessionCompleted:
                    await HandleCheckoutSessionCompleted(stripeEvent);
                    break;
                case EventTypes.CustomerSubscriptionUpdated:
                    await HandleSubscriptionUpdated(stripeEvent);
                    break;
                case EventTypes.CustomerSubscriptionDeleted:
                    await HandleSubscriptionDeleted(stripeEvent);
                    break;
                case EventTypes.InvoicePaymentFailed:
                    await HandlePaymentFailed(stripeEvent);
                    break;
            }
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe webhook error");
            throw;
        }
    }

    private async Task HandleCheckoutSessionCompleted(Event stripeEvent)
    {
        var session = stripeEvent.Data.Object as Session;
        if (session?.Metadata == null || !session.Metadata.TryGetValue("tenantId", out var tenantIdStr))
            return;

        if (!int.TryParse(tenantIdStr, out var tenantId)) return;

        var tenant = await _context.Tenants.FindAsync(tenantId);
        if (tenant == null) return;

        tenant.StripeCustomerId = session.CustomerId;
        tenant.StripeSubscriptionId = session.SubscriptionId;
        tenant.SubscriptionStatus = SubscriptionStatus.Active;

        // Get subscription details
        if (!string.IsNullOrEmpty(session.SubscriptionId))
        {
            var subService = new SubscriptionService();
            var subscription = await subService.GetAsync(session.SubscriptionId);
            tenant.CurrentPeriodEnd = subscription.CurrentPeriodEnd;
        }

        await _context.SaveChangesAsync();
    }

    private async Task HandleSubscriptionUpdated(Event stripeEvent)
    {
        var subscription = stripeEvent.Data.Object as Stripe.Subscription;
        if (subscription == null) return;

        var tenant = await _context.Tenants
            .FirstOrDefaultAsync(t => t.StripeSubscriptionId == subscription.Id);
        if (tenant == null) return;

        tenant.SubscriptionStatus = subscription.Status switch
        {
            "active" => SubscriptionStatus.Active,
            "past_due" => SubscriptionStatus.PastDue,
            "trialing" => SubscriptionStatus.Trialing,
            "incomplete" => SubscriptionStatus.Incomplete,
            _ => tenant.SubscriptionStatus
        };
        tenant.CurrentPeriodEnd = subscription.CurrentPeriodEnd;
        tenant.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    private async Task HandleSubscriptionDeleted(Event stripeEvent)
    {
        var subscription = stripeEvent.Data.Object as Stripe.Subscription;
        if (subscription == null) return;

        var tenant = await _context.Tenants
            .FirstOrDefaultAsync(t => t.StripeSubscriptionId == subscription.Id);
        if (tenant == null) return;

        tenant.SubscriptionStatus = SubscriptionStatus.Cancelled;
        tenant.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    private async Task HandlePaymentFailed(Event stripeEvent)
    {
        var invoice = stripeEvent.Data.Object as Invoice;
        if (invoice == null || string.IsNullOrEmpty(invoice.SubscriptionId)) return;

        var tenant = await _context.Tenants
            .FirstOrDefaultAsync(t => t.StripeSubscriptionId == invoice.SubscriptionId);
        if (tenant == null) return;

        tenant.SubscriptionStatus = SubscriptionStatus.PastDue;
        tenant.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }
}
