namespace DawaCloud.Web.Data.Entities;

/// <summary>
/// Represents a currency in the system (e.g., USD, SSP, KES)
/// </summary>
public class Currency : BaseAuditableEntity
{
    public int Id { get; set; }

    /// <summary>
    /// ISO 4217 currency code (e.g., "USD", "SSP", "KES")
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Full name of the currency (e.g., "US Dollar", "South Sudanese Pound")
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Currency symbol (e.g., "$", "SSP", "KES")
    /// </summary>
    public string Symbol { get; set; } = string.Empty;

    /// <summary>
    /// Number of decimal places for this currency (typically 2)
    /// </summary>
    public int DecimalPlaces { get; set; } = 2;

    /// <summary>
    /// Display format pattern (e.g., "{0} {1:N2}" for "SSP 1,000.00")
    /// </summary>
    public string Format { get; set; } = "{0} {1:N2}";

    /// <summary>
    /// Whether this currency is available for use
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Whether this is the base currency for the system (all values converted to this)
    /// Only one currency can be the base currency (SSP)
    /// </summary>
    public bool IsBaseCurrency { get; set; }

    /// <summary>
    /// Whether this is the default currency for purchases/procurement (USD)
    /// </summary>
    public bool IsDefaultPurchaseCurrency { get; set; }

    /// <summary>
    /// Whether this is the default currency for sales (SSP)
    /// </summary>
    public bool IsDefaultSalesCurrency { get; set; }

    /// <summary>
    /// Display order in currency selectors
    /// </summary>
    public int DisplayOrder { get; set; }
}

/// <summary>
/// Represents an exchange rate between two currencies
/// </summary>
public class ExchangeRate : BaseAuditableEntity
{
    public int Id { get; set; }

    /// <summary>
    /// Source currency ID (e.g., USD)
    /// </summary>
    public int FromCurrencyId { get; set; }

    /// <summary>
    /// Target currency ID (e.g., SSP)
    /// </summary>
    public int ToCurrencyId { get; set; }

    /// <summary>
    /// Exchange rate value (e.g., 1 USD = 850 SSP means Rate = 850)
    /// </summary>
    public decimal Rate { get; set; }

    /// <summary>
    /// Date from which this rate is effective
    /// </summary>
    public DateTime EffectiveDate { get; set; }

    /// <summary>
    /// Date when this rate expires (null = current active rate)
    /// </summary>
    public DateTime? ExpiryDate { get; set; }

    /// <summary>
    /// Optional notes about this rate change
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Whether this rate is currently active
    /// </summary>
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Currency FromCurrency { get; set; } = null!;
    public Currency ToCurrency { get; set; } = null!;
    public ICollection<ExchangeRateHistory> History { get; set; } = new List<ExchangeRateHistory>();
}

/// <summary>
/// Audit trail for exchange rate changes
/// </summary>
public class ExchangeRateHistory : BaseAuditableEntity
{
    public long Id { get; set; }

    /// <summary>
    /// The exchange rate that was changed
    /// </summary>
    public int ExchangeRateId { get; set; }

    /// <summary>
    /// Rate before the change
    /// </summary>
    public decimal PreviousRate { get; set; }

    /// <summary>
    /// Rate after the change
    /// </summary>
    public decimal NewRate { get; set; }

    /// <summary>
    /// When the change was made
    /// </summary>
    public DateTime ChangedAt { get; set; }

    /// <summary>
    /// Who made the change
    /// </summary>
    public string ChangedBy { get; set; } = string.Empty;

    /// <summary>
    /// Optional reason for the rate change
    /// </summary>
    public string? ChangeReason { get; set; }

    // Navigation property
    public ExchangeRate ExchangeRate { get; set; } = null!;
}
