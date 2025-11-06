namespace Manager.Infrastructure.Extensions
{
    public static class ExternalExtension
    {
        public static IServiceCollection AddExternalServices(this IServiceCollection services)
        {
            // 🔹 Registrar cliente HTTP
            services.AddHttpClient<MigeigvClient>();
            
            services.AddHttpClient<ClientesSolClient>();

            services.AddHttpClient<ControlAccesoClient>();
            services.AddHttpClient<CpeConsultaClient>();
            services.AddHttpClient<CpeControlClient>();

            // 🔹 Registrar SireComprasService con DI normal
            services.AddScoped<ISireComprasService, SireComprasService>();
            services.AddScoped<ICpeService, CpeService>();
            services.AddScoped<IClienteSolService, ClienteSolService>();

            return services;
        }
    }
}
