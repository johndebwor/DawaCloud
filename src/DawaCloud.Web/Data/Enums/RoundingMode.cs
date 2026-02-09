namespace DawaCloud.Web.Data.Enums;

/// <summary>
/// Defines how amounts should be rounded
/// </summary>
public enum RoundingMode
{
    /// <summary>
    /// Round to nearest value (e.g., 123 → 120 for value=10)
    /// </summary>
    Nearest,

    /// <summary>
    /// Always round up (e.g., 121 → 130 for value=10)
    /// </summary>
    Up,

    /// <summary>
    /// Always round down (e.g., 129 → 120 for value=10)
    /// </summary>
    Down
}
