using DawaFlow.Web.Data.Enums;

namespace DawaFlow.Web.Data.Entities;

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

    // Navigation properties
    public Currency? BaseCurrency { get; set; }
    public Currency? DefaultPurchaseCurrency { get; set; }
    public Currency? DefaultSalesCurrency { get; set; }
}
