using Microsoft.Extensions.DependencyInjection;

namespace Manager.Infrastructure.ExternalServices.Sire
{
    public class SireComprasService : ISireComprasService
    {
        private readonly MigeigvClient _migeigvClient;
        private readonly IServiceProvider _serviceProvider;

        public SireComprasService(MigeigvClient migeigvClient, IServiceProvider serviceProvider)
        {
            _migeigvClient = migeigvClient;
            _serviceProvider = serviceProvider;
        }

        public async Task<BaseResponseGeneric<ExportacionComprobantePropuestaResponse>> DescargarPropuestaRCEAsync(DescargarPropuestaRequest request, CancellationToken cancellationToken)
        {
            return await _migeigvClient.DescargarPropuestaRCEAsync(request, cancellationToken);
        }

        public async Task<BaseResponseGeneric<ConsultaEstadoTicketsResponse>> ConsultarEstadoTicketAsync(ConsultarEstadoTicketRequest request, CancellationToken cancellationToken)
        {
            return await _migeigvClient.ConsultarEstadoTicketAsync(request, cancellationToken);
        }

        public async Task<BaseResponseGeneric<DescargarArchivoReporteResponse>> DescargarArchivoReporteAsync(
            string token,
            Guid clienteId,
            DescargarArchivoReporteRequest request,
            CancellationToken cancellationToken)
        {
            var archivoResponse = await _migeigvClient.DescargarArchivoReporteAsync(token, request, cancellationToken);

            // ⚙️ Si hay error 2244 (ticket expirado), intentar uno nuevo automáticamente
            if (!archivoResponse.Success && EsError2244(archivoResponse.Data))
            {
                // ✅ Rompe el ciclo: resuelve TicketService solo en este punto
                var ticketService = _serviceProvider.GetRequiredService<ITicketService>();

                var nuevoTicket = await ticketService.GetOrGenerateActiveTicketAsync(token, new GetTicketRequest
                {
                    clienteId = clienteId,
                    perTributario = request.PerTributario
                });

                if (!nuevoTicket.Success)
                    return ResponseFactory.Error<DescargarArchivoReporteResponse>(nuevoTicket.Message, nuevoTicket.ErrorCode, nuevoTicket.StatusCode, nuevoTicket.Details);

                return await _migeigvClient.DescargarArchivoReporteAsync(token, new DescargarArchivoReporteRequest
                {
                    PerTributario = request.PerTributario,
                    NomArchivoReporte = nuevoTicket.Data!.NomArchivoReporte,
                    CodTipoArchivoReporte = nuevoTicket.Data!.CodTipoAchivoReporte,
                    NumTicket = nuevoTicket.Data!.NumTicket,
                    CodProceso = nuevoTicket.Data!.CodProceso
                }, cancellationToken);
            }

            return archivoResponse;
        }

        private static bool EsError2244(DescargarArchivoReporteResponse? archivoResponse)
        {
            var errorJson = archivoResponse?.ErrorContent;
            if (string.IsNullOrEmpty(errorJson)) return false;

            var parsedError = System.Text.Json.JsonSerializer.Deserialize<ArchivoReporteErrorMessage>(errorJson);
            return parsedError?.errors?.Any(e => e.cod == 2244) == true;
        }
    }
}
