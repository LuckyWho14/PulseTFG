using CommunityToolkit.Maui;
using Syncfusion.Maui.Core.Hosting;
using Microsoft.Extensions.Logging;

namespace PulseTFG
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Anton-Regular.ttf", "AntonRegular");
                })
                .UseMauiCommunityToolkit(); // <-- muévelo aquí

#if DEBUG
            builder.Logging.AddDebug();
            builder.ConfigureSyncfusionCore();

#endif

            return builder.Build();
        }

    }
}
