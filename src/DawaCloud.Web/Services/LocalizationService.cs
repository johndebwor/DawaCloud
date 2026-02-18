using System.Globalization;
using DawaCloud.Web.Data;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Services;

public class LocalizationService : ILocalizationService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public LocalizationService(
        IHttpContextAccessor httpContextAccessor,
        IServiceScopeFactory serviceScopeFactory)
    {
        _httpContextAccessor = httpContextAccessor;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public CultureInfo CurrentCulture =>
        _httpContextAccessor.HttpContext?.Features
            .Get<IRequestCultureFeature>()?.RequestCulture.Culture ?? CultureInfo.CurrentCulture;

    public bool IsRightToLeft => CurrentCulture.TextInfo.IsRightToLeft;

    public async Task<string> FormatCurrencyAsync(decimal amount, string? currencyCode = null)
    {
        currencyCode ??= "SSP"; // Default to South Sudanese Pound

        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var currency = await context.Currencies
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Code == currencyCode);

        if (currency == null)
        {
            // Fallback to culture-based currency formatting
            return amount.ToString("C", CurrentCulture);
        }

        // Use currency symbol and format from database
        var format = currency.Format ?? "N2";
        var formattedNumber = amount.ToString(format, CurrentCulture);

        // Build currency string: "SSP 1,234.56" or "1,234.56 SSP" depending on culture
        if (CurrentCulture.Name == "en-US" || CurrentCulture.Name == "en-GB")
        {
            return $"{currency.Symbol} {formattedNumber}";
        }
        else
        {
            return $"{formattedNumber} {currency.Symbol}";
        }
    }

    public string FormatNumber(decimal number, int decimals = 2)
    {
        var format = $"N{decimals}";
        return number.ToString(format, CurrentCulture);
    }

    public string FormatDate(DateTime date, string? format = null)
    {
        format ??= CurrentCulture.DateTimeFormat.ShortDatePattern;
        return date.ToString(format, CurrentCulture);
    }

    public string FormatDateTime(DateTime dateTime, string? format = null)
    {
        if (format == null)
        {
            var datePattern = CurrentCulture.DateTimeFormat.ShortDatePattern;
            var timePattern = CurrentCulture.DateTimeFormat.ShortTimePattern;
            format = $"{datePattern} {timePattern}";
        }

        return dateTime.ToString(format, CurrentCulture);
    }
}
