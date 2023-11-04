using Microsoft.AspNetCore.Components;
using QEBSPEditor.Models;
using QEBSPEditor.Services;

namespace QEBSPEditor.Components;

public class ApplicationSettingsWatcher : ComponentBase, IDisposable
{
    [Inject] public ApplicationSettingsService ApplicationSettings { get; set; } = default!;

    [Parameter] public EventCallback<ApplicationSettings> SettingsChange { get; set; }

    protected override void OnInitialized()
    {
        ApplicationSettings.Changed += OnApplicationSettingsChanged;
    }

    private void OnApplicationSettingsChanged(object? sender, Models.ApplicationSettings e)
    {
        SettingsChange.InvokeAsync(e);
    }

    public void Dispose()
    {
        ApplicationSettings.Changed -= OnApplicationSettingsChanged;
    }
}