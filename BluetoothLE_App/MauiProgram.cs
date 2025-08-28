using BluetoothLE_App.Helpers;
using Microsoft.Extensions.Logging;
using Shiny;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace BluetoothLE_App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                 .UseShiny()
                .UseSkiaSharp()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .RegisterShinyServices();
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
        static MauiAppBuilder RegisterShinyServices(this MauiAppBuilder builder)
        {
            var s = builder.Services;
#if IOS
        s.AddBluetoothLE(config:new Shiny.BluetoothLE.AppleBleConfiguration(true));
#elif ANDROID
            s.AddBluetoothLE();
#endif
            s.AddBleHostedCharacteristic<MyManagedCharacteristics>();
            s.AddBleHostedCharacteristic<MyManagedRequestCharacteristic>();

            return builder;
        }
    }
}
