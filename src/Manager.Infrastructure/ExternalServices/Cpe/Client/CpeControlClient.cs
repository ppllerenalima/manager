namespace Manager.Infrastructure.ExternalServices.Cpe.Client
{
    public class CpeControlClient
    {
        private readonly HttpClient _httpClient;

        public CpeControlClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<BaseResponseGeneric<DescargarZipResponse>> DescargarAsync(
            string token,
            DescargarZipRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ Construir la URL dinámica según los parámetros del comprobante
                var url = $"v1/contribuyente/controlcpe/consultaxml/{request.RucEmisor}-{request.TipoComprobante}-{request.Serie}-{request.Numero}";

                using var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                httpRequest.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
                httpRequest.Headers.Accept.ParseAdd("application/json");

                // 2️⃣ Enviar la solicitud con soporte de cancelación
                using var response = await _httpClient.SendAsync(
                    httpRequest,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken
                ).ConfigureAwait(false);

                var statusCode = (int)response.StatusCode;

                // 3️⃣ Si la respuesta es exitosa → leer el archivo ZIP
                if (response.IsSuccessStatusCode)
                {
                    var archivoBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);

                    var data = new DescargarZipResponse
                    {
                        Archivo = archivoBytes,
                        NombreArchivo = $"{request.RucEmisor}-{request.TipoComprobante}-{request.Serie}-{request.Numero}.zip"
                    };

                    return ResponseFactory.Success(data, "Archivo descargado correctamente desde ControlCPE.", statusCode);
                }

                // 4️⃣ Si hubo error HTTP → limpiar contenido de error y devolver respuesta estandarizada
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                errorContent = ExternalServiceHelper.CleanErrorContent(errorContent, statusCode);

                return ResponseFactory.Error<DescargarZipResponse>(
                    errorContent,
                    "EXTERNAL_SERVICE_ERROR",
                    statusCode
                );
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return ResponseFactory.Error<DescargarZipResponse>(
                    "La operación fue cancelada por el usuario o el sistema.",
                    "OPERATION_CANCELLED",
                    499 // HTTP 499 (Client Closed Request)
                );
            }
            catch (HttpRequestException ex)
            {
                return ResponseFactory.Error<DescargarZipResponse>(
                    $"Error de red: {ex.Message}",
                    "HTTP_REQUEST_EXCEPTION",
                    500
                );
            }
            catch (TaskCanceledException ex)
            {
                return ResponseFactory.Error<DescargarZipResponse>(
                    $"Timeout al conectar con SUNAT: {ex.Message}",
                    "TIMEOUT_EXCEPTION",
                    504
                );
            }
            catch (Exception ex)
            {
                return ResponseFactory.Error<DescargarZipResponse>(
                    $"Excepción no controlada: {ex.Message}",
                    "GENERAL_EXCEPTION",
                    500
                );
            }
        }
    }
}
