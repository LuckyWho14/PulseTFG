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

            // ✅ Registra tu licencia de Syncfusion aquí
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NNaF1cWWhOYVdpR2Nbek5yflVDal9QVBYiSV9jS3tTcEVnWHteeHdcQmNVUU90Vg=="); 

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Anton-Regular.ttf", "AntonRegular");
                })
                .UseMauiCommunityToolkit()
                .ConfigureSyncfusionCore(); // ✅ mueve esto fuera del #if DEBUG

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
