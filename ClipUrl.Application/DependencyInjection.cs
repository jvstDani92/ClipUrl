using ClipUrl.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ClipUrl.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IUrlService, UrlService>();

            return services;
        }
    }
}
