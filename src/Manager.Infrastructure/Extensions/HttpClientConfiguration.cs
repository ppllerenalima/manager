namespace Manager.Infrastructure.Extensions
{
    public static class HttpClientConfiguration
    {
        public static IServiceCollection AddExternalHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            var externalConfig = configuration.GetSection("ExternalServices");

            services.AddHttpClient<CpeControlClient>(client =>
            {
                client.BaseAddress = new Uri(externalConfig["CpeApiBaseUrl"]);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromSeconds(15);
            });

            services.AddHttpClient<CpeConsultaClient>(client =>
            {
                client.BaseAddress = new Uri(externalConfig["CpeApiBaseUrl"]);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromSeconds(15);
            });

            services.AddHttpClient<ControlAccesoClient>(client =>
            {
                client.BaseAddress = new Uri(externalConfig["SunatApiBaseUrl"]);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromSeconds(15);
            });

            services.AddHttpClient<MigeigvClient>(client =>
            {
                client.BaseAddress = new Uri(externalConfig["SireSunatApiBaseUrl"]);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromSeconds(15);
            });

            services.AddHttpClient<ClientesSolClient>(client =>
            {
                client.BaseAddress = new Uri(externalConfig["SeguridadSunatApiBaseUrl"]);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromSeconds(15);
            });

            return services;
        }
    }
}
