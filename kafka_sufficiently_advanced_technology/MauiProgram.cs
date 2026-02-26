using Microsoft.Extensions.Logging;
using kafka_sufficiently_advanced_technology.Services;
using kafka_sufficiently_advanced_technology.ViewModels;

namespace kafka_sufficiently_advanced_technology
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
                    fonts.AddFont("OpenSans-Regular.ttf",   "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf",  "OpenSansSemibold");
                });

            // Services
            builder.Services.AddSingleton<IKafkaService,        MockKafkaService>();
            builder.Services.AddSingleton<INugetBrowserService, MockNugetBrowserService>();

            // ViewModels
            builder.Services.AddSingleton<MainViewModel>();

            // Pages
            builder.Services.AddSingleton<MainPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
