using DawaCloud.Web.Data.Enums;

namespace DawaCloud.Web.Data.Entities;

/// <summary>
/// Company-wide settings and preferences
/// </summary>
public class CompanySettings : BaseAuditableEntity
{
    public int Id { get; set; }

    // Company Profile
    public string CompanyName { get; set; } = string.Empty;
    public string? PharmacyLicenseNumber { get; set; }
    public string? TaxId { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Website { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? LogoUrl { get; set; }
    public string? StampUrl { get; set; }
    public string DefaultCurrency { get; set; } = "SSP";
    public decimal DefaultTaxRate { get; set; } = 16;
    public int LowStockThresholdDays { get; set; } = 14;
    public int ExpiryAlertDays30 { get; set; } = 30;
    public int ExpiryAlertDays60 { get; set; } = 60;
    public int ExpiryAlertDays90 { get; set; } = 90;

    // Email/SMTP Settings
    public string? SmtpHost { get; set; }
    public int SmtpPort { get; set; } = 587;
    public string? SmtpUsername { get; set; }
    public string? SmtpPassword { get; set; }
    public string? FromEmail { get; set; }
    public string? FromName { get; set; }

    // WhatsApp/Twilio Settings
    public string? TwilioAccountSid { get; set; }
    public string? TwilioAuthToken { get; set; }
    public string? TwilioFromNumber { get; set; }

    // Currency Settings
    public int? BaseCurrencyId { get; set; }
    public int? DefaultPurchaseCurrencyId { get; set; }
    public int? DefaultSalesCurrencyId { get; set; }

    // Rounding Settings
    /// <summary>
    /// Rounding mode for SSP amounts (Nearest, Up, Down)
    /// </summary>
    public RoundingMode RoundingMode { get; set; } = RoundingMode.Nearest;

    /// <summary>
    /// Rounding value (10, 100, 1000)
    /// </summary>
    public int RoundingValue { get; set; } = 10;

    /// <summary>
    /// Whether automatic rounding is enabled
    /// </summary>
    public bool IsRoundingEnabled { get; set; } = false;

    /// <summary>
    /// Apply rounding to retail sales (POS)
    /// </summary>
    public bool ApplyRoundingToRetail { get; set; } = true;

    /// <summary>
    /// Apply rounding to wholesale sales
    /// </summary>
    public bool ApplyRoundingToWholesale { get; set; } = true;

    /// <summary>
    /// Apply rounding to invoices
    /// </summary>
    public bool ApplyRoundingToInvoices { get; set; } = true;

    // POS Settings
    /// <summary>
    /// POS product display style: "grid" or "list"
    /// </summary>
    public string PosProductDisplayStyle { get; set; } = "grid";

    // AI Chat Settings
    /// <summary>
    /// Whether AI Chat is enabled
    /// </summary>
    public bool AiChatEnabled { get; set; }

    /// <summary>
    /// Anthropic API Key for Claude AI
    /// </summary>
    public string? AiChatApiKey { get; set; }

    /// <summary>
    /// AI model to use (e.g., claude-sonnet-4-5-20250929)
    /// </summary>
    public string? AiChatModel { get; set; }

    /// <summary>
    /// Anthropic API endpoint URL
    /// </summary>
    public string? AiChatEndpoint { get; set; }

    // Localization Settings
    /// <summary>
    /// Default system locale (e.g., en-US, ar-SA)
    /// </summary>
    public string DefaultLocale { get; set; } = "en-US";

    /// <summary>
    /// Supported locales for the system (comma-separated)
    /// </summary>
    public string SupportedLocales { get; set; } = "en-US,ar-SA,fr-FR,de-DE,pt-PT,es-ES";

    /// <summary>
    /// Whether to enable automatic translation for missing strings
    /// </summary>
    public bool EnableAutoTranslation { get; set; } = false;

    // Navigation properties
    public Currency? BaseCurrency { get; set; }
    public Currency? DefaultPurchaseCurrency { get; set; }
    public Currency? DefaultSalesCurrency { get; set; }
}
