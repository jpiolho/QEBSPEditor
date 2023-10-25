using Microsoft.JSInterop;

namespace QEBSPEditor.Services;

public class ThemeService
{
    private static readonly string[] Themes = new string[]
    {
        "material",
        "standard",
        "default",
        "dark",
        "humanistic",
        "software"
    };

    private readonly IJSRuntime _jsRuntime;

    public ThemeService(IJSRuntime jsRuntime)
    {
        this._jsRuntime = jsRuntime;
    }

    public string[] GetThemes()
    {
        return Themes;
    }

    public async Task ChangeThemeAsync(string newTheme)
    {
        await _jsRuntime.InvokeVoidAsync("changeRadzenTheme", newTheme);
    }
}
