using DawaFlow.Web.Data;
using DawaFlow.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace DawaFlow.Web.Features.Currency.Services;

/// <summary>
/// Implementation of ICurrencyService for currency conversion and exchange rate management
/// </summary>
public class CurrencyService : ICurrencyService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CurrencyService> _logger;
    private readonly IRoundingService _roundingService;

    private const string CurrenciesCacheKey = "Currencies_All";
    private const string BaseCurrencyCacheKey = "Currency_Base";
    private const string ExchangeRatesCacheKey = "ExchangeRates_Active";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    // In-memory cache for currency formatting (currencies rarely change)
    private static Dictionary<int, CurrencyDto>? _currencyCache;
    private static Dictionary<string, int>? _currencyCodeToIdCache;

    public CurrencyService(IServiceScopeFactory scopeFactory, IMemoryCache cache, ILogger<CurrencyService> logger, IRoundingService roundingService)
    {
        _scopeFactory = scopeFactory;
        _cache = cache;
        _logger = logger;
        _roundingService = roundingService;
    }

    #region Currency Queries

    public async Task<List<CurrencyDto>> GetAllCurrenciesAsync(bool activeOnly = true, CancellationToken ct = default)
    {
        var cacheKey = $"{CurrenciesCacheKey}_{activeOnly}";

        if (_cache.TryGetValue(cacheKey, out List<CurrencyDto>? cached) && cached != null)
            return cached;

        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var query = context.Currencies.AsNoTracking();

        if (activeOnly)
            query = query.Where(c => c.IsActive);

        var currencies = await query
            .OrderBy(c => c.DisplayOrder)
            .Select(c => new CurrencyDto(
                c.Id,
                c.Code,
                c.Name,
                c.Symbol,
                c.DecimalPlaces,
                c.Format,
                c.IsActive,
                c.IsBaseCurrency,
                c.IsDefaultPurchaseCurrency,
                c.IsDefaultSalesCurrency,
                c.DisplayOrder
            ))
            .ToListAsync(ct);

        _cache.Set(cacheKey, currencies, CacheDuration);

        // Update static cache for formatting
        _currencyCache = currencies.ToDictionary(c => c.Id);
        _currencyCodeToIdCache = currencies.ToDictionary(c => c.Code, c => c.Id);

        return currencies;
    }

    public async Task<CurrencyDto?> GetCurrencyByCodeAsync(string code, CancellationToken ct = default)
    {
        var currencies = await GetAllCurrenciesAsync(false, ct);
        return currencies.FirstOrDefault(c => c.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<CurrencyDto?> GetCurrencyByIdAsync(int id, CancellationToken ct = default)
    {
        var currencies = await GetAllCurrenciesAsync(false, ct);
        return currencies.FirstOrDefault(c => c.Id == id);
    }

    public async Task<CurrencyDto> GetBaseCurrencyAsync(CancellationToken ct = default)
    {
        if (_cache.TryGetValue(BaseCurrencyCacheKey, out CurrencyDto? cached) && cached != null)
            return cached;

        var currencies = await GetAllCurrenciesAsync(true, ct);
        var baseCurrency = currencies.FirstOrDefault(c => c.IsBaseCurrency)
            ?? throw new InvalidOperationException("No base currency configured in the system");

        _cache.Set(BaseCurrencyCacheKey, baseCurrency, CacheDuration);
        return baseCurrency;
    }

    public async Task<CurrencyDto> GetDefaultPurchaseCurrencyAsync(CancellationToken ct = default)
    {
        var currencies = await GetAllCurrenciesAsync(true, ct);
        return currencies.FirstOrDefault(c => c.IsDefaultPurchaseCurrency)
            ?? await GetBaseCurrencyAsync(ct);
    }

    public async Task<CurrencyDto> GetDefaultSalesCurrencyAsync(CancellationToken ct = default)
    {
        var currencies = await GetAllCurrenciesAsync(true, ct);
        return currencies.FirstOrDefault(c => c.IsDefaultSalesCurrency)
            ?? await GetBaseCurrencyAsync(ct);
    }

    #endregion

    #region Exchange Rate Queries

    public async Task<decimal> GetCurrentRateAsync(int fromCurrencyId, int toCurrencyId, CancellationToken ct = default)
    {
        if (fromCurrencyId == toCurrencyId)
            return 1m;

        var rate = await GetExchangeRateAsync(fromCurrencyId, toCurrencyId, ct);
        if (rate != null)
            return rate.Rate;

        // Try reverse rate
        var reverseRate = await GetExchangeRateAsync(toCurrencyId, fromCurrencyId, ct);
        if (reverseRate != null && reverseRate.Rate != 0)
            return 1m / reverseRate.Rate;

        _logger.LogWarning("No exchange rate found between currency {From} and {To}", fromCurrencyId, toCurrencyId);
        throw new InvalidOperationException($"No exchange rate found between currency {fromCurrencyId} and {toCurrencyId}");
    }

    public async Task<decimal> GetCurrentRateAsync(string fromCode, string toCode, CancellationToken ct = default)
    {
        var fromCurrency = await GetCurrencyByCodeAsync(fromCode, ct)
            ?? throw new ArgumentException($"Currency not found: {fromCode}", nameof(fromCode));
        var toCurrency = await GetCurrencyByCodeAsync(toCode, ct)
            ?? throw new ArgumentException($"Currency not found: {toCode}", nameof(toCode));

        return await GetCurrentRateAsync(fromCurrency.Id, toCurrency.Id, ct);
    }

    public async Task<ExchangeRateDto?> GetExchangeRateAsync(int fromCurrencyId, int toCurrencyId, CancellationToken ct = default)
    {
        var rates = await GetAllExchangeRatesAsync(true, ct);
        return rates.FirstOrDefault(r => r.FromCurrencyId == fromCurrencyId && r.ToCurrencyId == toCurrencyId);
    }

    public async Task<List<ExchangeRateDto>> GetAllExchangeRatesAsync(bool activeOnly = true, CancellationToken ct = default)
    {
        var cacheKey = $"{ExchangeRatesCacheKey}_{activeOnly}";

        if (_cache.TryGetValue(cacheKey, out List<ExchangeRateDto>? cached) && cached != null)
            return cached;

        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var query = context.ExchangeRates
            .Include(er => er.FromCurrency)
            .Include(er => er.ToCurrency)
            .AsNoTracking();

        if (activeOnly)
            query = query.Where(er => er.IsActive && (er.ExpiryDate == null || er.ExpiryDate > DateTime.UtcNow));

        var rates = await query
            .OrderBy(er => er.FromCurrency.Code)
            .ThenBy(er => er.ToCurrency.Code)
            .Select(er => new ExchangeRateDto(
                er.Id,
                er.FromCurrencyId,
                er.FromCurrency.Code,
                er.FromCurrency.Name,
                er.ToCurrencyId,
                er.ToCurrency.Code,
                er.ToCurrency.Name,
                er.Rate,
                er.EffectiveDate,
                er.ExpiryDate,
                er.Notes,
                er.IsActive
            ))
            .ToListAsync(ct);

        _cache.Set(cacheKey, rates, CacheDuration);
        return rates;
    }

    #endregion

    #region Conversion

    public async Task<decimal> ConvertAsync(decimal amount, string fromCurrency, string toCurrency, CancellationToken ct = default)
    {
        if (fromCurrency.Equals(toCurrency, StringComparison.OrdinalIgnoreCase))
            return amount;

        var rate = await GetCurrentRateAsync(fromCurrency, toCurrency, ct);
        var converted = amount * rate;

        // Apply rounding if converting TO base currency (SSP)
        var baseCurrency = await GetBaseCurrencyAsync(ct);
        if (toCurrency.Equals(baseCurrency.Code, StringComparison.OrdinalIgnoreCase))
        {
            return await _roundingService.ApplyRoundingAsync(converted, "Conversion", ct);
        }

        return Math.Round(converted, 2);
    }

    public async Task<decimal> ConvertAsync(decimal amount, int fromCurrencyId, int toCurrencyId, CancellationToken ct = default)
    {
        if (fromCurrencyId == toCurrencyId)
            return amount;

        var rate = await GetCurrentRateAsync(fromCurrencyId, toCurrencyId, ct);
        var converted = amount * rate;

        // Apply rounding if converting TO base currency (SSP)
        var baseCurrency = await GetBaseCurrencyAsync(ct);
        if (toCurrencyId == baseCurrency.Id)
        {
            return await _roundingService.ApplyRoundingAsync(converted, "Conversion", ct);
        }

        return Math.Round(converted, 2);
    }

    public async Task<decimal> ConvertToBaseAsync(decimal amount, string fromCurrency, CancellationToken ct = default)
    {
        var baseCurrency = await GetBaseCurrencyAsync(ct);
        return await ConvertAsync(amount, fromCurrency, baseCurrency.Code, ct);
    }

    public async Task<decimal> ConvertToBaseAsync(decimal amount, int fromCurrencyId, CancellationToken ct = default)
    {
        var baseCurrency = await GetBaseCurrencyAsync(ct);
        return await ConvertAsync(amount, fromCurrencyId, baseCurrency.Id, ct);
    }

    public async Task<decimal> ConvertFromBaseAsync(decimal amount, string toCurrency, CancellationToken ct = default)
    {
        var baseCurrency = await GetBaseCurrencyAsync(ct);
        return await ConvertAsync(amount, baseCurrency.Code, toCurrency, ct);
    }

    public async Task<decimal> ConvertFromBaseAsync(decimal amount, int toCurrencyId, CancellationToken ct = default)
    {
        var baseCurrency = await GetBaseCurrencyAsync(ct);
        return await ConvertAsync(amount, baseCurrency.Id, toCurrencyId, ct);
    }

    #endregion

    #region Formatting

    public string Format(decimal amount, string currencyCode)
    {
        if (_currencyCodeToIdCache != null && _currencyCodeToIdCache.TryGetValue(currencyCode, out var id))
            return Format(amount, id);

        // Fallback to simple format
        return $"{currencyCode} {amount:N2}";
    }

    public string Format(decimal amount, int currencyId)
    {
        if (_currencyCache != null && _currencyCache.TryGetValue(currencyId, out var currency))
        {
            return string.Format(currency.Format, currency.Symbol, amount);
        }

        // Fallback to simple format
        return $"{amount:N2}";
    }

    public string FormatWithSymbol(decimal amount, string currencyCode)
    {
        if (_currencyCodeToIdCache != null && _currencyCodeToIdCache.TryGetValue(currencyCode, out var id))
            return FormatWithSymbol(amount, id);

        return $"{currencyCode} {amount:N2}";
    }

    public string FormatWithSymbol(decimal amount, int currencyId)
    {
        if (_currencyCache != null && _currencyCache.TryGetValue(currencyId, out var currency))
        {
            return string.Format(currency.Format, currency.Symbol, amount);
        }

        return $"{amount:N2}";
    }

    #endregion

    #region Exchange Rate Management

    public async Task<SetExchangeRateResult> SetExchangeRateAsync(
        int fromCurrencyId,
        int toCurrencyId,
        decimal rate,
        string? notes,
        string changedBy,
        CancellationToken ct = default)
    {
        if (rate <= 0)
            return new SetExchangeRateResult(false, "Exchange rate must be greater than zero");

        if (fromCurrencyId == toCurrencyId)
            return new SetExchangeRateResult(false, "From and To currencies must be different");

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Check if currencies exist
            var fromCurrency = await context.Currencies.FindAsync(new object[] { fromCurrencyId }, ct);
            var toCurrency = await context.Currencies.FindAsync(new object[] { toCurrencyId }, ct);

            if (fromCurrency == null || toCurrency == null)
                return new SetExchangeRateResult(false, "One or both currencies not found");

            // Find existing active rate
            var existingRate = await context.ExchangeRates
                .FirstOrDefaultAsync(er =>
                    er.FromCurrencyId == fromCurrencyId &&
                    er.ToCurrencyId == toCurrencyId &&
                    er.IsActive, ct);

            if (existingRate != null)
            {
                // Record history
                var history = new ExchangeRateHistory
                {
                    ExchangeRateId = existingRate.Id,
                    PreviousRate = existingRate.Rate,
                    NewRate = rate,
                    ChangedAt = DateTime.UtcNow,
                    ChangedBy = changedBy,
                    ChangeReason = notes,
                    CreatedAt = DateTime.UtcNow
                };
                context.ExchangeRateHistories.Add(history);

                // Update existing rate
                existingRate.Rate = rate;
                existingRate.EffectiveDate = DateTime.UtcNow;
                existingRate.Notes = notes;
                existingRate.UpdatedAt = DateTime.UtcNow;
                existingRate.UpdatedBy = changedBy;
            }
            else
            {
                // Create new rate
                existingRate = new ExchangeRate
                {
                    FromCurrencyId = fromCurrencyId,
                    ToCurrencyId = toCurrencyId,
                    Rate = rate,
                    EffectiveDate = DateTime.UtcNow,
                    Notes = notes,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = changedBy
                };
                context.ExchangeRates.Add(existingRate);
            }

            await context.SaveChangesAsync(ct);

            // Clear cache (use bool.ToString() to match the cache key format)
            _cache.Remove($"{ExchangeRatesCacheKey}_{true}");
            _cache.Remove($"{ExchangeRatesCacheKey}_{false}");

            _logger.LogInformation("Exchange rate updated: {From} to {To} = {Rate} by {User}",
                fromCurrency.Code, toCurrency.Code, rate, changedBy);

            return new SetExchangeRateResult(true, "Exchange rate updated successfully", existingRate.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting exchange rate from {From} to {To}", fromCurrencyId, toCurrencyId);
            return new SetExchangeRateResult(false, $"Error updating exchange rate: {ex.Message}");
        }
    }

    public async Task<List<ExchangeRateHistoryDto>> GetExchangeRateHistoryAsync(int exchangeRateId, int? limit = null, CancellationToken ct = default)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var query = context.ExchangeRateHistories
            .AsNoTracking()
            .Where(h => h.ExchangeRateId == exchangeRateId)
            .OrderByDescending(h => h.ChangedAt);

        if (limit.HasValue)
            query = (IOrderedQueryable<ExchangeRateHistory>)query.Take(limit.Value);

        return await query
            .Select(h => new ExchangeRateHistoryDto(
                h.Id,
                h.ExchangeRateId,
                h.PreviousRate,
                h.NewRate,
                h.ChangedAt,
                h.ChangedBy,
                h.ChangeReason
            ))
            .ToListAsync(ct);
    }

    #endregion
}
