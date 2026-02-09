using DawaFlow.Web.Data;
using DawaFlow.Web.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace DawaFlow.Web.Features.Currency.Services;

public interface IRoundingService
{
    Task<decimal> ApplyRoundingAsync(decimal amount, string context, CancellationToken ct = default);
    decimal ApplyRounding(decimal amount, RoundingMode mode, int roundingValue);
    Task<RoundingSettings> GetRoundingSettingsAsync(CancellationToken ct = default);
}

public class RoundingService : IRoundingService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMemoryCache _cache;
    private const string CacheKey = "RoundingSettings";
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(5);

    public RoundingService(IServiceScopeFactory scopeFactory, IMemoryCache cache)
    {
        _scopeFactory = scopeFactory;
        _cache = cache;
    }

    public async Task<decimal> ApplyRoundingAsync(decimal amount, string context, CancellationToken ct = default)
    {
        var settings = await GetRoundingSettingsAsync(ct);

        if (!settings.IsEnabled) return amount;

        // Check context-specific settings
        bool shouldRound = context switch
        {
            "Retail" => settings.ApplyToRetail,
            "Wholesale" => settings.ApplyToWholesale,
            "Invoice" => settings.ApplyToInvoices,
            "Conversion" => true, // Always apply to currency conversions to SSP
            _ => false
        };

        if (!shouldRound) return amount;

        return ApplyRounding(amount, settings.Mode, settings.RoundingValue);
    }

    public decimal ApplyRounding(decimal amount, RoundingMode mode, int roundingValue)
    {
        if (roundingValue <= 1) return amount;

        return mode switch
        {
            RoundingMode.Nearest => Math.Round(amount / roundingValue) * roundingValue,
            RoundingMode.Up => Math.Ceiling(amount / roundingValue) * roundingValue,
            RoundingMode.Down => Math.Floor(amount / roundingValue) * roundingValue,
            _ => amount
        };
    }

    public async Task<RoundingSettings> GetRoundingSettingsAsync(CancellationToken ct = default)
    {
        if (_cache.TryGetValue(CacheKey, out RoundingSettings? cachedSettings) && cachedSettings != null)
        {
            return cachedSettings;
        }

        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var companySettings = await context.CompanySettings.FirstOrDefaultAsync(ct);

        var settings = new RoundingSettings
        {
            IsEnabled = companySettings?.IsRoundingEnabled ?? false,
            Mode = companySettings?.RoundingMode ?? RoundingMode.Nearest,
            RoundingValue = companySettings?.RoundingValue ?? 10,
            ApplyToRetail = companySettings?.ApplyRoundingToRetail ?? true,
            ApplyToWholesale = companySettings?.ApplyRoundingToWholesale ?? true,
            ApplyToInvoices = companySettings?.ApplyRoundingToInvoices ?? true
        };

        _cache.Set(CacheKey, settings, CacheExpiration);
        return settings;
    }
}

public record RoundingSettings
{
    public bool IsEnabled { get; init; }
    public RoundingMode Mode { get; init; }
    public int RoundingValue { get; init; }
    public bool ApplyToRetail { get; init; }
    public bool ApplyToWholesale { get; init; }
    public bool ApplyToInvoices { get; init; }
}
