using MudBlazor;

namespace DawaCloud.Web.Shared.Extensions;

public static class MudThemeConfiguration
{
    private static readonly string[] FontFamily = ["DM Sans", "system-ui", "-apple-system", "sans-serif"];

    public static MudTheme DawaFlowTheme => new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#0C8599",
            PrimaryDarken = "#0A7285",
            PrimaryLighten = "#1A9EAF",
            Secondary = "#10B981",
            SecondaryDarken = "#059669",
            SecondaryLighten = "#34D399",
            Tertiary = "#0C8599",
            Background = "#F8FAFB",
            Surface = "#FFFFFF",
            AppbarBackground = "#FFFFFF",
            AppbarText = "#1E293B",
            DrawerBackground = "#FFFFFF",
            DrawerText = "#64748B",
            DrawerIcon = "#94A3B8",
            TextPrimary = "#1E293B",
            TextSecondary = "#64748B",
            TextDisabled = "#94A3B8",
            ActionDefault = "#64748B",
            ActionDisabled = "#94A3B8",
            ActionDisabledBackground = "#F1F5F9",
            Divider = "#E2E8F0",
            DividerLight = "#F1F5F9",
            TableLines = "#F1F5F9",
            TableStriped = "#F8FAFB",
            TableHover = "#E6F7F9",
            LinesDefault = "#E2E8F0",
            LinesInputs = "#E2E8F0",
            Success = "#10B981",
            Warning = "#F59E0B",
            Error = "#EF4444",
            Info = "#0C8599",
            Dark = "#1E293B",
            OverlayDark = "rgba(0,0,0,0.5)",
            OverlayLight = "rgba(255,255,255,0.5)"
        },
        PaletteDark = new PaletteDark
        {
            Primary = "#14B8A6",
            PrimaryDarken = "#0D9488",
            PrimaryLighten = "#2DD4BF",
            Secondary = "#34D399",
            SecondaryDarken = "#10B981",
            SecondaryLighten = "#6EE7B7",
            Tertiary = "#14B8A6",
            Background = "#0C0A09",
            Surface = "#1C1917",
            AppbarBackground = "#1C1917",
            AppbarText = "#FAFAF9",
            DrawerBackground = "#1C1917",
            DrawerText = "#A8A29E",
            DrawerIcon = "#78716C",
            TextPrimary = "#FAFAF9",
            TextSecondary = "#A8A29E",
            TextDisabled = "#78716C",
            ActionDefault = "#A8A29E",
            ActionDisabled = "#78716C",
            ActionDisabledBackground = "#292524",
            Divider = "#3D3835",
            DividerLight = "#292524",
            TableLines = "#292524",
            TableStriped = "#1C1917",
            TableHover = "#0D1B1C",
            LinesDefault = "#3D3835",
            LinesInputs = "#3D3835",
            Success = "#34D399",
            Warning = "#FB923C",
            Error = "#FB7185",
            Info = "#2DD4BF",
            Dark = "#0C0A09",
            OverlayDark = "rgba(0,0,0,0.75)",
            OverlayLight = "rgba(255,255,255,0.04)"
        },
        Typography = new Typography
        {
            Default = new DefaultTypography { FontFamily = FontFamily },
            H1 = new H1Typography { FontFamily = FontFamily },
            H2 = new H2Typography { FontFamily = FontFamily },
            H3 = new H3Typography { FontFamily = FontFamily },
            H4 = new H4Typography { FontFamily = FontFamily },
            H5 = new H5Typography { FontFamily = FontFamily },
            H6 = new H6Typography { FontFamily = FontFamily },
            Subtitle1 = new Subtitle1Typography { FontFamily = FontFamily },
            Subtitle2 = new Subtitle2Typography { FontFamily = FontFamily },
            Body1 = new Body1Typography { FontFamily = FontFamily },
            Body2 = new Body2Typography { FontFamily = FontFamily },
            Button = new ButtonTypography { FontFamily = FontFamily },
            Caption = new CaptionTypography { FontFamily = FontFamily },
            Overline = new OverlineTypography { FontFamily = FontFamily }
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
