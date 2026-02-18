using System.Globalization;

namespace DawaCloud.Web.Services;

public interface ILocalizationService
{
    /// <summary>
    /// Formats a currency amount according to the current culture
    /// </summary>
    Task<string> FormatCurrencyAsync(decimal amount, string? currencyCode = null);

    /// <summary>
    /// Formats a number according to the current culture
    /// </summary>
    string FormatNumber(decimal number, int decimals = 2);

    /// <summary>
    /// Formats a date according to the current culture or specified format
    /// </summary>
    string FormatDate(DateTime date, string? format = null);

    /// <summary>
    /// Formats a date and time according to the current culture or specified format
    /// </summary>
    string FormatDateTime(DateTime dateTime, string? format = null);

    /// <summary>
    /// Gets the current culture for the request
    /// </summary>
    CultureInfo CurrentCulture { get; }

    /// <summary>
    /// Checks if the current culture uses right-to-left text direction
    /// </summary>
    bool IsRightToLeft { get; }
}
