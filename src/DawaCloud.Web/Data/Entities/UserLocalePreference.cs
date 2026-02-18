using DawaCloud.Web.Infrastructure.Identity;

namespace DawaCloud.Web.Data.Entities;

public class UserLocalePreference : BaseAuditableEntity
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string PreferredLocale { get; set; } = "en-US";
    public string DateFormat { get; set; } = "dd/MM/yyyy";
    public string TimeFormat { get; set; } = "HH:mm";
    public bool Use24HourFormat { get; set; } = true;

    // Navigation
    public ApplicationUser User { get; set; } = null!;
}
