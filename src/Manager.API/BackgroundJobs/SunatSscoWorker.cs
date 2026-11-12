namespace Manager.API.BackgroundJobs
{
    public class SunatSscoWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SunatSscoWorker> _logger;

        public SunatSscoWorker(IServiceScopeFactory scopeFactory, ILogger<SunatSscoWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Espera breve al arrancar el contenedor para asegurar que la API y BD estén listas
            await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            await EjecutarSincronizacionAsync(stoppingToken);

            // Revisión periódica cada 12 horas
            using var timer = new PeriodicTimer(TimeSpan.FromHours(12));
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await EjecutarSincronizacionAsync(stoppingToken);
            }
        }

        private async Task EjecutarSincronizacionAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var sunatService = scope.ServiceProvider.GetRequiredService<ISunatSscoService>();
                var resultado = await sunatService.SincronizarPadronAsync(cancellationToken);

                _logger.LogInformation("Ejecución automática SSCO: {Mensaje}", resultado.Mensaje);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la sincronización automática del padrón SSCO.");
            }
        }
    }
}
