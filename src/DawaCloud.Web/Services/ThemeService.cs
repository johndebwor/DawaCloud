using Microsoft.JSInterop;

namespace DawaCloud.Web.Services;

public class ThemeService
{
    private readonly IJSRuntime _jsRuntime;
    private bool _isDarkMode;
    private bool _initialized;

    public event Action? OnThemeChanged;

    public bool IsDarkMode => _isDarkMode;

    public ThemeService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task InitializeAsync()
    {
        if (_initialized) return;

        try
        {
            var preference = await _jsRuntime.InvokeAsync<string>("themeManager.getPreference");
            _isDarkMode = preference == "dark";
            _initialized = true;
        }
        catch
        {
            // JS interop may not be available during prerendering
            _isDarkMode = false;
        }
    }

    public async Task ToggleAsync()
    {
        _isDarkMode = !_isDarkMode;
        await _jsRuntime.InvokeVoidAsync("themeManager.setTheme", _isDarkMode);
        OnThemeChanged?.Invoke();
    }

    public async Task SetDarkModeAsync(bool isDark)
    {
        _isDarkMode = isDark;
        await _jsRuntime.InvokeVoidAsync("themeManager.setTheme", isDark);
        OnThemeChanged?.Invoke();
    }
}
