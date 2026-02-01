namespace DawaFlow.Web.Features.Currency.Services;

/// <summary>
/// Service for currency conversion, formatting, and exchange rate management
/// </summary>
public interface ICurrencyService
{
    // Currency queries
    Task<List<CurrencyDto>> GetAllCurrenciesAsync(bool activeOnly = true, CancellationToken ct = default);
    Task<CurrencyDto?> GetCurrencyByCodeAsync(string code, CancellationToken ct = default);
    Task<CurrencyDto?> GetCurrencyByIdAsync(int id, CancellationToken ct = default);
    Task<CurrencyDto> GetBaseCurrencyAsync(CancellationToken ct = default);
    Task<CurrencyDto> GetDefaultPurchaseCurrencyAsync(CancellationToken ct = default);
    Task<CurrencyDto> GetDefaultSalesCurrencyAsync(CancellationToken ct = default);

    // Exchange rate queries
    Task<decimal> GetCurrentRateAsync(int fromCurrencyId, int toCurrencyId, CancellationToken ct = default);
    Task<decimal> GetCurrentRateAsync(string fromCode, string toCode, CancellationToken ct = default);
    Task<ExchangeRateDto?> GetExchangeRateAsync(int fromCurrencyId, int toCurrencyId, CancellationToken ct = default);
    Task<List<ExchangeRateDto>> GetAllExchangeRatesAsync(bool activeOnly = true, CancellationToken ct = default);

    // Conversion
    Task<decimal> ConvertAsync(decimal amount, string fromCurrency, string toCurrency, CancellationToken ct = default);
    Task<decimal> ConvertAsync(decimal amount, int fromCurrencyId, int toCurrencyId, CancellationToken ct = default);
    Task<decimal> ConvertToBaseAsync(decimal amount, string fromCurrency, CancellationToken ct = default);
    Task<decimal> ConvertToBaseAsync(decimal amount, int fromCurrencyId, CancellationToken ct = default);
    Task<decimal> ConvertFromBaseAsync(decimal amount, string toCurrency, CancellationToken ct = default);
    Task<decimal> ConvertFromBaseAsync(decimal amount, int toCurrencyId, CancellationToken ct = default);

    // Formatting
    string Format(decimal amount, string currencyCode);
    string Format(decimal amount, int currencyId);
    string FormatWithSymbol(decimal amount, string currencyCode);
    string FormatWithSymbol(decimal amount, int currencyId);

    // Exchange rate management
    Task<SetExchangeRateResult> SetExchangeRateAsync(int fromCurrencyId, int toCurrencyId, decimal rate, string? notes, string changedBy, CancellationToken ct = default);
    Task<List<ExchangeRateHistoryDto>> GetExchangeRateHistoryAsync(int exchangeRateId, int? limit = null, CancellationToken ct = default);
}

// DTOs
public record CurrencyDto(
    int Id,
    string Code,
    string Name,
    string Symbol,
    int DecimalPlaces,
    string Format,
    bool IsActive,
    bool IsBaseCurrency,
    bool IsDefaultPurchaseCurrency,
    bool IsDefaultSalesCurrency,
    int DisplayOrder
);

public record ExchangeRateDto(
    int Id,
    int FromCurrencyId,
    string FromCurrencyCode,
    string FromCurrencyName,
    int ToCurrencyId,
    string ToCurrencyCode,
    string ToCurrencyName,
    decimal Rate,
    DateTime EffectiveDate,
    DateTime? ExpiryDate,
    string? Notes,
    bool IsActive
);

public record ExchangeRateHistoryDto(
    long Id,
    int ExchangeRateId,
    decimal PreviousRate,
    decimal NewRate,
    DateTime ChangedAt,
    string ChangedBy,
    string? ChangeReason
);

public record SetExchangeRateResult(
    bool Success,
    string Message,
    int? ExchangeRateId = null
);

public record CurrencyConversionResult(
    decimal OriginalAmount,
    string OriginalCurrency,
    decimal ConvertedAmount,
    string TargetCurrency,
    decimal ExchangeRate,
    DateTime RateDate
);
