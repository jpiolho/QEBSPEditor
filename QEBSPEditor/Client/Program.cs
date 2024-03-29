using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using QEBSPEditor.Services;
using Radzen;
using System.Text;
using Tewr.Blazor.FileReader;

namespace QEBSPEditor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddSweetAlert2();
            builder.Services.AddBlazorDownloadFile();

            builder.Services.AddScoped<AlertDialogsService>();
            builder.Services.AddScoped<DialogService>();
            builder.Services.AddScoped<TooltipService>();
            builder.Services.AddScoped<ApplicationSettingsService>();
            builder.Services.AddScoped<ThemeService>();
            builder.Services.AddScoped<BSPService>();

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddFileReaderService(options => options.UseWasmSharedBuffer = true);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            await builder.Build().RunAsync();
        }
    }
}