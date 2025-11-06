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
                // 🔹 Determinar sufijo según tipo
                // Tipo: "01" → ZIP con PDF, "02" → ZIP con XML, vacío → solo JSON (consulta)
                var tipoSufijo = string.IsNullOrEmpty(request.Tipo)
                    ? string.Empty
                    : $"/{request.Tipo}";

                // 🔹 Determinar procedencia (-1 = venta, -2 = compra)
                var procedencia = request.EsVenta ? "-1" : "-2";

                // 🔹 Construcción dinámica de URL
                var url = $"v1/contribuyente/consultacpe/comprobantes/{request.RucEmisor}-{request.TipoComprobante}-{request.Serie}-{request.Numero}{procedencia}{tipoSufijo}";

                using var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                httpRequest.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
                httpRequest.Headers.Accept.ParseAdd("application/json");

                using var response = await _httpClient
                    .SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                    .ConfigureAwait(false);

                var statusCode = (int)response.StatusCode;
                var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

                // ❌ Manejo de error HTTP
                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = ExternalServiceHelper.CleanErrorContent(content, statusCode);
                    return ResponseFactory.Error<DescargarZipResponse>(
                        errorMessage,
                        "EXTERNAL_SERVICE_ERROR",
                        statusCode
                    );
                }

                // ⚠️ Si la respuesta está vacía
                if (string.IsNullOrWhiteSpace(content))
                {
                    return ResponseFactory.Error<DescargarZipResponse>(
                        "La respuesta del servicio está vacía.",
                        "EMPTY_RESPONSE",
                        statusCode
                    );
                }

                // 🔍 Caso 1: si se solicitó ZIP (PDF o XML)
                if (!string.IsNullOrEmpty(request.Tipo))
                {
                    // El servicio devuelve Base64
                    ConsultaCpeComprobanteResponse? resultado;
                    try
                    {
                        resultado = JsonConvert.DeserializeObject<ConsultaCpeComprobanteResponse>(content);
                    }
                    catch (Newtonsoft.Json.JsonException ex)
                    {
                        return ResponseFactory.Error<DescargarZipResponse>(
                            $"Error al deserializar la respuesta ZIP: {ex.Message}",
                            "DESERIALIZATION_ERROR",
                            statusCode
                        );
                    }

                    if (resultado == null)
                    {
                        return ResponseFactory.Error<DescargarZipResponse>(
                            "No se pudo deserializar la respuesta ZIP del servicio.",
                            "NULL_RESPONSE_OBJECT",
                            statusCode
                        );
                    }

                    var archivoBytes = string.IsNullOrEmpty(resultado.ValArchivo)
                        ? Array.Empty<byte>()
                        : Convert.FromBase64String(resultado.ValArchivo);

                    var nombreArchivo = resultado.NomArchivo ??
                        (request.Tipo == "01" ? "archivo_pdf.zip" :
                         request.Tipo == "02" ? "archivo_xml.zip" : "archivo.zip");

                    var data = new DescargarZipResponse
                    {
                        Tipo = request.Tipo,
                        Archivo = archivoBytes,
                        NombreArchivo = nombreArchivo
                    };

                    return ResponseFactory.Success(
                        data,
                        $"Archivo descargado correctamente ({(request.Tipo == "01" ? "PDF" : "XML")}).",
                        statusCode
                    );
                }

                // 🔍 Caso 2: solo consulta (JSON estructurado)
                try
                {
                    var resultadoJson = JsonConvert.DeserializeObject<ConsultaCpeConsultaResponse>(content);

                    if (resultadoJson == null)
                    {
                        return ResponseFactory.Error<DescargarZipResponse>(
                            "No se pudo deserializar la respuesta de consulta.",
                            "NULL_JSON_RESPONSE",
                            statusCode
                        );
                    }

                    // Mapeo ligero a DescargarZipResponse (opcional)
                    var data = new DescargarZipResponse
                    {
                        NombreArchivo = "consulta.json",
                        Archivo = System.Text.Encoding.UTF8.GetBytes(content)
                    };

                    return ResponseFactory.Success(
                        data,
                        "Consulta de comprobante realizada correctamente.",
                        statusCode
                    );
                }
                catch (Newtonsoft.Json.JsonException ex)
                {
                    return ResponseFactory.Error<DescargarZipResponse>(
                        $"Error al deserializar la respuesta de consulta: {ex.Message}",
                        "DESERIALIZATION_ERROR",
                        statusCode
                    );
                }
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
