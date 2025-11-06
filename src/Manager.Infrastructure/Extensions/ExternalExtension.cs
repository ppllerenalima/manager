namespace Manager.Infrastructure.Extensions
{
    public static class ExternalExtension
    {
        public static IServiceCollection AddExternalServices(this IServiceCollection services)
        {
            services.AddHttpClient<ISireComprasService, SireComprasService>();
            services.AddScoped<ICpeService, CpeService>();
            services.AddScoped<IClienteSolService, ClienteSolService>();

            return services;
        }
    }
}
