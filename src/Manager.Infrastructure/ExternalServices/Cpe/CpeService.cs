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

        public async Task<BaseResponseGeneric<DescargarZipResponse>> DescargarZipAsync(
            string token,
            DescargarZipRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                // 🔹 1️⃣ Intentar con ControlCpe
                var control = await _controlClient.DescargarAsync(token, request, cancellationToken);
                if (control.Success && control.Data != null)
                    return control;

                // 🔹 2️⃣ Intentar con ConsultaCpe (ZIP + XML)
                var requestXml = new DescargarZipRequest
                {
                    RucEmisor = request.RucEmisor,
                    TipoComprobante = request.TipoComprobante,
                    Serie = request.Serie,
                    Numero = request.Numero,
                    EsVenta = false,
                    Tipo = "02" // ZIP con XML
                };

                var consultaXml = await _consultaClient.DescargarAsync(token, requestXml, cancellationToken);
                if (consultaXml.Success && consultaXml.Data != null)
                    return consultaXml;

                // 🔹 3️⃣ Intentar con ConsultaCpe (ZIP + XML)
                var requestJson = new DescargarZipRequest
                {
                    RucEmisor = request.RucEmisor,
                    TipoComprobante = request.TipoComprobante,
                    Serie = request.Serie,
                    Numero = request.Numero,
                    EsVenta = false,
                    Tipo = null // JSON
                };

                var consultaJson = await _consultaClient.DescargarAsync(token, requestJson, cancellationToken);
                if (consultaJson.Success && consultaJson.Data != null)
                    return consultaJson;

                // 🔹 2️⃣ Intentar con ConsultaCpe (ZIP + PDF)
                var requesPdf = new DescargarZipRequest
                {
                    RucEmisor = request.RucEmisor,
                    TipoComprobante = request.TipoComprobante,
                    Serie = request.Serie,
                    Numero = request.Numero,
                    EsVenta = false,
                    Tipo = "01" // ZIP con PDF
                };

                var consultaPdf = await _consultaClient.DescargarAsync(token, requesPdf, cancellationToken);
                if (consultaPdf.Success && consultaPdf.Data != null)
                    return consultaPdf;

                // 🔹 4️⃣ Si todos fallan
                var mensajeError =
                    "No se pudo descargar el archivo desde ninguno de los servicios disponibles.\n\n" +
                    $"🔸 ControlCpe ZIP+XML: {control.Message} (Status {control.StatusCode})\n" +
                    $"🔸 ConsultaCpe ZIP+XML: {consultaXml.Message} (Status {consultaXml.StatusCode})\n" +
                    $"🔸 ConsultaCpe JSON: {consultaJson.Message} (Status {consultaJson.StatusCode})\n" +
                    $"🔸 ConsultaCpe PDF: {consultaPdf.Message} (Status {consultaPdf.StatusCode})";

                return ResponseFactory.Error<DescargarZipResponse>(
                    mensajeError,
                    "ZIP_DOWNLOAD_FAILED",
                    502 // Bad Gateway: error al comunicarse con servicios externos
                );
            }
            catch (Exception ex)
            {
                return ResponseFactory.Error<DescargarZipResponse>(
                    $"Error inesperado en DescargarZipAsync: {ex.Message}",
                    "ZIP_DOWNLOAD_EXCEPTION",
                    500
                );
            }
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
