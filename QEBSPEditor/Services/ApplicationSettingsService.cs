using Microsoft.JSInterop;
using QEBSPEditor.Models;
using System.Text.Json;

namespace QEBSPEditor.Services
{
    public class ApplicationSettingsService
    {
        public event EventHandler<ApplicationSettings>? Changed;
        public ApplicationSettings Settings => _settings;

        private ApplicationSettings _settings = new();
        private readonly IJSRuntime _jsRuntime;

        public ApplicationSettingsService(IJSRuntime JSRuntime)
        {
            _jsRuntime = JSRuntime;
        }


        public async Task LoadAsync()
        {
            var raw = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "settings");
            if (raw is null)
                return;

            var settings = JsonSerializer.Deserialize<ApplicationSettings>(raw);
            if (settings is null)
                return;

            _settings = settings;
            Changed?.Invoke(this, _settings);
        }

        public async Task SaveAsync(ApplicationSettings settings)
        {
            _settings = settings;

            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "settings", JsonSerializer.Serialize(settings));

            Changed?.Invoke(this, settings);
        }

    }
}
