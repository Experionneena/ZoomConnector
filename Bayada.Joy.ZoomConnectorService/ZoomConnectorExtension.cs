using Bayada.Joy.ZoomConnector.ConfigOptions;
using Bayada.Joy.ZoomConnector.Contracts;
using Bayada.Joy.ZoomConnector.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Bayada.Joy.ZoomConnector.Extensions
{
    public static class ZoomConnectorExtension
    {
        public static void RegisterDependencies(this IServiceCollection services, Action<ZoomSettings> zoomSettings)
        {
            services.Configure(zoomSettings);
            services.AddScoped<IZoomService, ZoomService>();
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); 
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
        }
    }
}
