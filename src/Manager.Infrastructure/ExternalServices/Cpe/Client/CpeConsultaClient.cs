namespace Manager.Infrastructure.ExternalServices.Cpe.Client
{
    public class CpeConsultaClient
    {
        private readonly HttpClient _httpClient;

        public CpeConsultaClient(HttpClient httpClient)
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
                // 1️⃣ Construcción de URL
                var url = $"v1/contribuyente/consultacpe/comprobantes/{request.RucEmisor}-{request.TipoComprobante}-{request.Serie}-{request.Numero}-2/{request.Tipo}";

                using var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                httpRequest.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/138.0.0.0 Safari/537.36 Edg/138.0.0.0");
                httpRequest.Headers.Accept.ParseAdd("application/json");

                // 2️⃣ Enviar la solicitud con cancelación
                using var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                var statusCode = (int)response.StatusCode;

                var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

                // 3️⃣ Manejo de error HTTP
                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = ExternalServiceHelper.CleanErrorContent(content, statusCode);
                    return ResponseFactory.Error<DescargarZipResponse>(
                        errorMessage,
                        "EXTERNAL_SERVICE_ERROR",
                        statusCode
                    );
                }

                if (string.IsNullOrWhiteSpace(content))
                {
                    return ResponseFactory.Error<DescargarZipResponse>(
                        "La respuesta del servicio está vacía.",
                        "EMPTY_RESPONSE",
                        statusCode
                    );
                }

                // 4️⃣ Deserializar
                ConsultaCpeComprobanteResponse? resultado;
                try
                {
                    resultado = JsonConvert.DeserializeObject<ConsultaCpeComprobanteResponse>(content);
                }
                catch (System.Text.Json.JsonException ex)
                {
                    return ResponseFactory.Error<DescargarZipResponse>(
                        $"Error al deserializar la respuesta: {ex.Message}",
                        "DESERIALIZATION_ERROR",
                        statusCode
                    );
                }

                if (resultado == null)
                {
                    return ResponseFactory.Error<DescargarZipResponse>(
                        "No se pudo deserializar la respuesta del servicio.",
                        "NULL_RESPONSE_OBJECT",
                        statusCode
                    );
                }

                // 5️⃣ Convertir base64 a bytes
                var archivoBytes = string.IsNullOrEmpty(resultado.ValArchivo)
                    ? Array.Empty<byte>()
                    : Convert.FromBase64String(resultado.ValArchivo);

                var data = new DescargarZipResponse
                {
                    Archivo = archivoBytes,
                    NombreArchivo = resultado.NomArchivo ?? "archivo.zip"
                };

                return ResponseFactory.Success(data, "Archivo descargado correctamente.", statusCode);
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
                // Esto captura timeouts
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
