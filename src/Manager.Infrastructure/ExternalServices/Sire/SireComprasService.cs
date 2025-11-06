namespace Manager.Infrastructure.ExternalServices.Sire
{
    public class SireComprasService : ISireComprasService
    {
        private readonly MigeigvClient _migeigvClient;

        public SireComprasService(MigeigvClient migeigvClient)
        {
            _migeigvClient = migeigvClient;
        }

        public async Task<BaseResponseGeneric<ExportacionComprobantePropuestaResponse>> DescargarPropuestaRCEAsync(DescargarPropuestaRequest request, CancellationToken cancellationToken)
        {
            return await _migeigvClient.DescargarPropuestaRCEAsync(request, cancellationToken);
        }

        public async Task<BaseResponseGeneric<ConsultaEstadoTicketsResponse>> ConsultarEstadoTicketAsync(ConsultarEstadoTicketRequest request, CancellationToken cancellationToken)
        {
            return await _migeigvClient.ConsultarEstadoTicketAsync(request, cancellationToken);
        }

        public async Task<BaseResponseGeneric<DescargarArchivoReporteResponse>> DescargarArchivoReporteAsync(string token, DescargarArchivoReporteRequest request, CancellationToken cancellationToken)
        {
            return await _migeigvClient.DescargarArchivoReporteAsync(token, request, cancellationToken);
        }
    }
}
