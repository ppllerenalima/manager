
namespace Manager.Infrastructure.ExternalServices.Cpe
{
    public class CpeService : ICpeService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://api-cpe.sunat.gob.pe/";

        public CpeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(BaseUrl);
        }

        public async Task<BaseResponseGeneric<DescargarZipResponse>> DescargarZipAsync(string token, DescargarZipRequest request)
        {
            // 1️⃣ Intentar primero con ControlCpe
            var resultadoControl = await DescargarPorControlCpeAsync(token, request);

            if (resultadoControl.Success && resultadoControl.Data != null)
                return ResponseFactory.Success(resultadoControl.Data, "Archivo descargado correctamente desde ControlCpe.", resultadoControl.StatusCode);

            // 2️⃣ Si falló, intentar con ConsultaCpe
            var resultadoConsulta = await DescargarPorConsultaCpeAsync(token, request);

            if (resultadoConsulta.Success && resultadoConsulta.Data != null)
                return ResponseFactory.Success(resultadoConsulta.Data, "Archivo descargado correctamente desde ConsultaCpe.", resultadoConsulta.StatusCode);

            // 3️⃣ Si ambos fallaron → combinar mensajes de error
            var mensajeError =
                $"No se pudo descargar el archivo desde ninguno de los servicios. " +
                $"ControlCpe: {resultadoControl.Message ?? "Error desconocido"}. " +
                $"ConsultaCpe: {resultadoConsulta.Message ?? "Error desconocido"}.";

            return ResponseFactory.Error<DescargarZipResponse>(mensajeError, "ZIP_DOWNLOAD_FAILED", 500);
        }

        public async Task<BaseResponseGeneric<string>> ActualizarPermisosAsync(string token, ActualizarPermisosRequest request)
        {
            try
            {
                var url = "https://api.sunat.gob.pe/v1/tecnologia/controlacceso/aplicaciones";

                var body = new
                {
                    id = request.Id,
                    expFlujoAutoriz = "100000",
                    nomApp = request.NomApp,
                    desUrlApp = request.DesUrlApp,
                    recursos = new[]
                    {
                        new { desPathRecurso = "/v1/contribuyente/controlcpe" },
                        new { desPathRecurso = "/v1/contribuyente/migeigv" },
                        new { desPathRecurso = "/v1/contribuyente/gem" },
                        new { desPathRecurso = "/v1/contribuyente/consultacpe" },
                        new { desPathRecurso = "/v1/contribuyente/gre" },
                        new { desPathRecurso = "/v1/contribuyente/parametros" },
                        new { desPathRecurso = "/v1/contribuyente/contribuyentes" },
                        new { desPathRecurso = "/v1/contribuyente/consultacpe/parametros" }
                    }
                };

                var json = JsonConvert.SerializeObject(body);
                using var httpRequest = new HttpRequestMessage(HttpMethod.Put, url);
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                httpRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");

                using var response = await _httpClient.SendAsync(httpRequest);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                    return ResponseFactory.Success(content, "Permisos actualizados correctamente.", (int)response.StatusCode);

                return ResponseFactory.Error<string>(content, "EXTERNAL_SERVICE_ERROR", (int)response.StatusCode);
            }
            catch (HttpRequestException ex)
            {
                return ResponseFactory.Error<string>($"Error de red: {ex.Message}", "HTTP_REQUEST_EXCEPTION", 500);
            }
            catch (Exception ex)
            {
                return ResponseFactory.Error<string>($"Excepción: {ex.Message}", "EXCEPTION", 500);
            }
        }

        #region PRIVADOS
        private async Task<BaseResponseGeneric<DescargarZipResponse>> DescargarPorControlCpeAsync(string token, DescargarZipRequest request)
        {
            try
            {
                var url = $"v1/contribuyente/controlcpe/consultaxml/{request.RucEmisor}-{request.TipoComprobante}-{request.Serie}-{request.Numero}";
                using var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                using var response = await _httpClient.SendAsync(httpRequest);
                var statusCode = (int)response.StatusCode;

                if (response.IsSuccessStatusCode)
                {
                    var data = new DescargarZipResponse
                    {
                        Archivo = await response.Content.ReadAsByteArrayAsync(),
                        NombreArchivo = $"{request.RucEmisor}-{request.TipoComprobante}-{request.Serie}-{request.Numero}"
                    };

                    return ResponseFactory.Success(data, "Archivo descargado correctamente", statusCode);
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                errorContent = ExternalServiceHelper.CleanErrorContent(errorContent, statusCode);

                return ResponseFactory.Error<DescargarZipResponse>(
                    errorContent,
                    "EXTERNAL_SERVICE_ERROR",
                    statusCode
                );
            }
            catch (Exception ex)
            {
                return ResponseFactory.Error<DescargarZipResponse>($"Excepción: {ex.Message}", "EXCEPTION", 500);
            }
        }

        public async Task<BaseResponseGeneric<DescargarZipResponse>> DescargarPorConsultaCpeAsync(string token, DescargarZipRequest request)
        {
            try
            {
                // Construcción de URL
                var url = $"v1/contribuyente/consultacpe/comprobantes/{request.RucEmisor}-{request.TipoComprobante}-{request.Serie}-{request.Numero}-2/{request.Tipo}";

                using var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                httpRequest.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/138.0.0.0 Safari/537.36 Edg/138.0.0.0");
                httpRequest.Headers.Accept.ParseAdd("application/json");

                // Envía la solicitud
                using var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                var statusCode = (int)response.StatusCode;

                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

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
        #endregion
    }
}
