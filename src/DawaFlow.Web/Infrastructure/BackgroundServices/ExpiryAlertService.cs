using DawaFlow.Web.Data;
using DawaFlow.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DawaFlow.Web.Infrastructure.BackgroundServices;

public class ExpiryAlertService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ExpiryAlertService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24);

    public ExpiryAlertService(
        IServiceScopeFactory scopeFactory,
        ILogger<ExpiryAlertService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Expiry Alert Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessExpiryAlertsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing expiry alerts");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }

    private async Task ProcessExpiryAlertsAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var expiringBatches = await context.Batches
            .Where(b => b.ExpiryDate <= DateTime.UtcNow.AddDays(90))
            .Where(b => b.Status == BatchStatus.Active)
            .Include(b => b.Drug)
            .ToListAsync(ct);

        if (expiringBatches.Any())
        {
            _logger.LogWarning(
                "Found {Count} batches expiring within 90 days",
                expiringBatches.Count);

            foreach (var batch in expiringBatches)
            {
                var daysUntilExpiry = (batch.ExpiryDate - DateTime.UtcNow).Days;
                _logger.LogInformation(
                    "Batch {BatchNumber} of {DrugName} expires in {Days} days",
                    batch.BatchNumber,
                    batch.Drug.Name,
                    daysUntilExpiry);

                // Here you would send notifications via email/WhatsApp
                // await _notificationService.SendExpiryAlertAsync(batch, ct);
            }
        }
    }
}
