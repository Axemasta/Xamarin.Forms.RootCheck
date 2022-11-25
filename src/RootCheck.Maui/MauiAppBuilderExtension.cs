using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Hosting;

namespace RootCheck.Maui
{
    public static class MauiAppBuilderExtension
    {
        public static MauiAppBuilder UseRootCheck(this MauiAppBuilder builder)
        {
            var rootChecker = RootChecker.Current;

            builder.Services.AddSingleton(rootChecker);

            return builder;
        }
    }
}
