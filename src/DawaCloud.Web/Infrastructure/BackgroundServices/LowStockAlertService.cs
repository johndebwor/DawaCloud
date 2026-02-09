using DawaCloud.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Infrastructure.BackgroundServices;

public class LowStockAlertService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<LowStockAlertService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(6);

    public LowStockAlertService(
        IServiceScopeFactory scopeFactory,
        ILogger<LowStockAlertService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Low Stock Alert Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckLowStockAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking low stock");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }

    private async Task CheckLowStockAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var lowStockDrugs = await context.Drugs
            .Include(d => d.Batches)
            .Where(d => d.IsActive)
            .ToListAsync(ct);

        var alerts = lowStockDrugs
            .Where(d => d.Batches.Sum(b => b.CurrentQuantity) <= d.ReorderLevel)
            .ToList();

        if (alerts.Any())
        {
            _logger.LogWarning(
                "Found {Count} drugs with low stock",
                alerts.Count);

            foreach (var drug in alerts)
            {
                var currentStock = drug.Batches.Sum(b => b.CurrentQuantity);
                _logger.LogInformation(
                    "Low stock alert: {DrugName} - Current: {Current}, Reorder Level: {ReorderLevel}",
                    drug.Name,
                    currentStock,
                    drug.ReorderLevel);

                // Here you would send notifications
                // await _notificationService.SendLowStockAlertAsync(drug, ct);
            }
        }
    }
}
