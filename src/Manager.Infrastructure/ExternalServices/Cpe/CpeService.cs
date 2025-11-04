namespace Manager.Infrastructure.ExternalServices.Cpe
{
    public class CpeService : ICpeService
    {
        private readonly CpeControlClient _controlClient;
        private readonly CpeConsultaClient _consultaClient;
        private readonly ControlAccesoClient _controlAccesoClient;


        public CpeService(CpeControlClient controlClient, CpeConsultaClient consultaClient, ControlAccesoClient controlAccesoClient)
        {
            _controlClient = controlClient;
            _consultaClient = consultaClient;
            _controlAccesoClient = controlAccesoClient;
        }

        public async Task<BaseResponseGeneric<DescargarZipResponse>> DescargarZipAsync(string token, DescargarZipRequest request, CancellationToken cancellationToken)
        {
            // 1️⃣ Intentar con ControlCpe
            var control = await _controlClient.DescargarAsync(token, request, cancellationToken);
            if (control.Success && control.Data != null)
                return control;

            // 2️⃣ Si falla, probar con ConsultaCpe
            var consulta = await _consultaClient.DescargarAsync(token, request, cancellationToken);
            if (consulta.Success && consulta.Data != null)
                return consulta;

            // 3️⃣ Si ambos fallan
            var mensajeError = $"No se pudo descargar el archivo. ControlCpe: {control.Message}. ConsultaCpe: {consulta.Message}";
            return ResponseFactory.Error<DescargarZipResponse>(mensajeError, "ZIP_DOWNLOAD_FAILED", 500);
        }

        public async Task<BaseResponseGeneric<DescargarZipResponse>> DescargarPorConsultaCpeAsync(string token, DescargarZipRequest request, CancellationToken cancellationToken)
        { 
            return await _consultaClient.DescargarAsync(token, request, cancellationToken);
        }

        public async Task<BaseResponseGeneric<string>> ActualizarPermisosAsync(string token, ActualizarPermisosRequest request, CancellationToken cancellationToken)
        {
            return await _controlAccesoClient.ActualizarPermisosAsync(token, request, cancellationToken);
        }
    }
}
