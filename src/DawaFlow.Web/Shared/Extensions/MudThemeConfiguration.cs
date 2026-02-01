using MudBlazor;

namespace DawaFlow.Web.Shared.Extensions;

public static class MudThemeConfiguration
{
    public static MudTheme DawaFlowTheme => new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#1976d2",
            PrimaryDarken = "#0d47a1",
            PrimaryLighten = "#64b5f6",
            Secondary = "#ffb300",
            SecondaryDarken = "#ff8f00",
            SecondaryLighten = "#ffc947",
            Tertiary = "#1e88e5",
            Background = "#f5f7fa",
            Surface = "#ffffff",
            AppbarBackground = "#ffffff",
            DrawerBackground = "#ffffff",
            DrawerText = "#1a1a2e",
            TextPrimary = "#1a1a2e",
            TextSecondary = "#64748b",
            Success = "#43a047",
            Warning = "#fb8c00",
            Error = "#e53935",
            Info = "#1e88e5",
            Dark = "#1a1a2e"
        },
        PaletteDark = new PaletteDark
        {
            Primary = "#42a5f5",
            PrimaryDarken = "#1565c0",
            PrimaryLighten = "#90caf9",
            Secondary = "#ffc947",
            SecondaryDarken = "#ffb300",
            SecondaryLighten = "#ffd54f",
            Tertiary = "#64b5f6",
            Background = "#1a1a2e",
            Surface = "#16213e",
            AppbarBackground = "#16213e",
            DrawerBackground = "#16213e",
            DrawerText = "#e4e4e7",
            TextPrimary = "#e4e4e7",
            TextSecondary = "#a1a1aa",
            Success = "#66bb6a",
            Warning = "#ffa726",
            Error = "#ef5350",
            Info = "#42a5f5",
            Dark = "#0f1419"
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "12px",
            DrawerWidthLeft = "260px",
            DrawerWidthRight = "300px",
            AppbarHeight = "64px"
        },
        ZIndex = new ZIndex
        {
            Drawer = 1100,
            AppBar = 1200,
            Dialog = 1300,
            Popover = 1400,
            Snackbar = 1500,
            Tooltip = 1600
        }
    };
}
