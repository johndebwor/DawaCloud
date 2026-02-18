using DawaCloud.Web.Features.Subscription.Services;

namespace DawaCloud.Web.Features.Subscription;

public static class StripeWebhookEndpoint
{
    public static void MapStripeEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/stripe/webhook", async (HttpContext context, IStripeService stripeService) =>
        {
            var json = await new StreamReader(context.Request.Body).ReadToEndAsync();
            var signature = context.Request.Headers["Stripe-Signature"].FirstOrDefault();

            if (string.IsNullOrEmpty(signature))
                return Results.BadRequest("Missing Stripe signature");

            try
            {
                await stripeService.HandleWebhookAsync(json, signature);
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }).DisableAntiforgery();
    }
}
