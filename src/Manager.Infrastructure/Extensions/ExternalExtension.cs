using Manager.Domain.Services;
using Manager.Infrastructure.ExternalServices.Cpe;
using Manager.Infrastructure.ExternalServices.Sire;
using Microsoft.Extensions.DependencyInjection;

namespace Manager.Infrastructure.Extensions
{
    public static class ExternalExtension
    {
        public static IServiceCollection AddExternalServices(this IServiceCollection services)
        {
            services.AddHttpClient<ISireComprasService, SireComprasService>();

            services.AddScoped<ICpeService, CpeService>();

            return services;
        }
    }
}
