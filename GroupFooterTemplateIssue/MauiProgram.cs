using CommunityToolkit.Maui;
using GroupFooterTemplateIssue.Controls;
using GroupFooterTemplateIssue.Platforms.Windows;
using Microsoft.Extensions.Logging;

namespace GroupFooterTemplateIssue
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .ConfigureMauiHandlers(handlers =>
                {
#if WINDOWS
                    handlers.AddHandler(typeof(GroupedCollection), typeof(CollectionHandler));
#endif
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
