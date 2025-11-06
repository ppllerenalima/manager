namespace Manager.Infrastructure.ExternalServices.Sire.Client
{
    public class MigeigvClient
    {
        private readonly HttpClient _httpClient;

        public MigeigvClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<BaseResponseGeneric<ConsultaEstadoTicketsResponse>> ConsultarEstadoTicketAsync(
            ConsultarEstadoTicketRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ Construcción de URL
                var url = $"v1/contribuyente/migeigv/libros/rvierce/gestionprocesosmasivos/web/masivo/consultaestadotickets" +
                          $"?perIni={request.PerIni}&perFin={request.PerFin}&page={request.Page}&perPage={request.PerPage}";

                if (!string.IsNullOrEmpty(request.NumTicket))
                    url += $"&numTicket={request.NumTicket}";

                using var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", request.AccessToken);
                httpRequest.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/138.0.0.0 Safari/537.36");
                httpRequest.Headers.Accept.ParseAdd("application/json");

                // 2️⃣ Enviar la solicitud con cancelación
                using var response = await _httpClient
                    .SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                    .ConfigureAwait(false);

                var statusCode = (int)response.StatusCode;
                var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

                // 3️⃣ Validar respuesta HTTP
                if (!response.IsSuccessStatusCode)
                {
                    string errorMessage;

                    if (statusCode == 422)
                    {
                        try
                        {
                            var errorResponse = JsonConvert.DeserializeObject<Error422Response>(content);
                            errorMessage = errorResponse != null && errorResponse.Errors.Any()
                                ? string.Join("; ", errorResponse.Errors.Select(e => $"{e.Cod}: {e.Msg}"))
                                : "Error 422 sin detalles específicos.";
                        }
                        catch (Exception ex)
                        {
                            errorMessage = $"No se pudo interpretar el error 422: {ex.Message}";
                        }
                    }
                    else
                    {
                        errorMessage = ExternalServiceHelper.CleanErrorContent(content, statusCode);
                    }

                    return ResponseFactory.Error<ConsultaEstadoTicketsResponse>(
                        errorMessage,
                        "EXTERNAL_SERVICE_ERROR",
                        statusCode
                    );
                }

                // 4️⃣ Validar contenido
                if (string.IsNullOrWhiteSpace(content))
                {
                    return ResponseFactory.Error<ConsultaEstadoTicketsResponse>(
                        "La respuesta del servicio está vacía.",
                        "EMPTY_RESPONSE",
                        statusCode
                    );
                }

                // 5️⃣ Deserializar
                ConsultaEstadoTicketsResponse? resultado;
                try
                {
                    resultado = JsonConvert.DeserializeObject<ConsultaEstadoTicketsResponse>(content);
                }
                catch (Newtonsoft.Json.JsonException ex)
                {
                    return ResponseFactory.Error<ConsultaEstadoTicketsResponse>(
                        $"Error al deserializar la respuesta: {ex.Message}",
                        "DESERIALIZATION_ERROR",
                        statusCode
                    );
                }

                if (resultado == null)
                {
                    return ResponseFactory.Error<ConsultaEstadoTicketsResponse>(
                        "No se pudo deserializar la respuesta del servicio.",
                        "NULL_RESPONSE_OBJECT",
                        statusCode
                    );
                }

                // 6️⃣ Marcar como éxito
                return ResponseFactory.Success(
                    resultado,
                    "Consulta de estado de ticket realizada correctamente.",
                    statusCode
                );
            }
            catch (HttpRequestException ex)
            {
                return ResponseFactory.Error<ConsultaEstadoTicketsResponse>(
                    $"Error de red: {ex.Message}",
                    "HTTP_REQUEST_EXCEPTION",
                    500
                );
            }
            catch (TaskCanceledException ex)
            {
                return ResponseFactory.Error<ConsultaEstadoTicketsResponse>(
                    $"Timeout al conectar con SUNAT: {ex.Message}",
                    "TIMEOUT_EXCEPTION",
                    504
                );
            }
            catch (Exception ex)
            {
                return ResponseFactory.Error<ConsultaEstadoTicketsResponse>(
                    $"Excepción no controlada: {ex.Message}",
                    "GENERAL_EXCEPTION",
                    500
                );
            }
        }

        public async Task<BaseResponseGeneric<ExportacionComprobantePropuestaResponse>> DescargarPropuestaRCEAsync(
            DescargarPropuestaRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ Construcción de parámetros
                var queryParams = new Dictionary<string, string>
                {
                    ["codTipoArchivo"] = request.CodTipoArchivo.ToString(),
                    ["codOrigenEnvio"] = request.CodOrigenEnvio,
                    ["codTipoCDP"] = request.CodTipoCDP
                };

                if (!string.IsNullOrWhiteSpace(request.FecEmisionIni)) queryParams["fecEmisionIni"] = request.FecEmisionIni;
                if (!string.IsNullOrWhiteSpace(request.FecEmisionFin)) queryParams["fecEmisionFin"] = request.FecEmisionFin;
                if (!string.IsNullOrWhiteSpace(request.CodInconsistencia)) queryParams["codInconsistencia"] = request.CodInconsistencia;
                if (!string.IsNullOrWhiteSpace(request.CodCar)) queryParams["codCar"] = request.CodCar;
                if (!string.IsNullOrWhiteSpace(request.NumDocAdquiriente)) queryParams["numDocAdquiriente"] = request.NumDocAdquiriente;

                if (request.MtoDesde.HasValue)
                    queryParams["mtoDesde"] = request.MtoDesde.Value.ToString("F2", CultureInfo.InvariantCulture);
                if (request.MtoHasta.HasValue)
                    queryParams["mtoHasta"] = request.MtoHasta.Value.ToString("F2", CultureInfo.InvariantCulture);

                var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));

                // 2️⃣ Construcción de URL completa
                var url = $"v1/contribuyente/migeigv/libros/rce/propuesta/web/propuesta/{request.PerTributario}/exportacioncomprobantepropuesta?{queryString}";

                using var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", request.Token);
                httpRequest.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
                httpRequest.Headers.Accept.ParseAdd("application/json");

                // 3️⃣ Envío de solicitud con cancelación
                using var response = await _httpClient
                    .SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                    .ConfigureAwait(false);

                var statusCode = (int)response.StatusCode;
                var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

                // 4️⃣ Manejo de errores HTTP
                if (!response.IsSuccessStatusCode)
                {
                    string errorMessage;

                    try
                    {
                        var errorObj = JsonConvert.DeserializeObject<SunatErrorResponse>(content);
                        if (errorObj?.Errors != null && errorObj.Errors.Count > 0)
                        {
                            errorMessage = string.Join("; ", errorObj.Errors.Select(e => $"{e.Cod}: {e.Msg}"));
                        }
                        else
                        {
                            errorMessage = errorObj?.Msg ?? $"Error {statusCode}: {response.ReasonPhrase}";
                        }
                    }
                    catch
                    {
                        errorMessage = $"Error {statusCode}: {response.ReasonPhrase}";
                    }

                    return ResponseFactory.Error<ExportacionComprobantePropuestaResponse>(
                        errorMessage,
                        "EXTERNAL_SERVICE_ERROR",
                        statusCode
                    );
                }

                // 5️⃣ Validar contenido vacío
                if (string.IsNullOrWhiteSpace(content))
                {
                    return ResponseFactory.Error<ExportacionComprobantePropuestaResponse>(
                        "La respuesta del servicio está vacía.",
                        "EMPTY_RESPONSE",
                        statusCode
                    );
                }

                // 6️⃣ Deserializar respuesta exitosa
                ExportacionComprobantePropuestaResponse? sunatResponse;
                try
                {
                    sunatResponse = JsonConvert.DeserializeObject<ExportacionComprobantePropuestaResponse>(content);
                }
                catch (Newtonsoft.Json.JsonException ex)
                {
                    return ResponseFactory.Error<ExportacionComprobantePropuestaResponse>(
                        $"Error al deserializar la respuesta: {ex.Message}",
                        "DESERIALIZATION_ERROR",
                        statusCode
                    );
                }

                if (sunatResponse == null)
                {
                    return ResponseFactory.Error<ExportacionComprobantePropuestaResponse>(
                        "No se pudo deserializar la respuesta del servicio.",
                        "NULL_RESPONSE_OBJECT",
                        statusCode
                    );
                }

                // 7️⃣ Construir respuesta final
                var data = new ExportacionComprobantePropuestaResponse
                {
                    NumTicket = sunatResponse.NumTicket
                };

                return ResponseFactory.Success(data, "Propuesta descargada correctamente.", statusCode);
            }
            catch (HttpRequestException ex)
            {
                return ResponseFactory.Error<ExportacionComprobantePropuestaResponse>(
                    $"Error de red: {ex.Message}",
                    "HTTP_REQUEST_EXCEPTION",
                    500
                );
            }
            catch (TaskCanceledException ex)
            {
                return ResponseFactory.Error<ExportacionComprobantePropuestaResponse>(
                    $"Timeout al conectar con SUNAT: {ex.Message}",
                    "TIMEOUT_EXCEPTION",
                    504
                );
            }
            catch (Exception ex)
            {
                return ResponseFactory.Error<ExportacionComprobantePropuestaResponse>(
                    $"Excepción no controlada: {ex.Message}",
                    "GENERAL_EXCEPTION",
                    500
                );
            }
        }

        public async Task<BaseResponseGeneric<DescargarArchivoReporteResponse>> DescargarArchivoReporteAsync(
            string token,
            DescargarArchivoReporteRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ Construcción de parámetros de consulta
                var queryParams = new Dictionary<string, string>
                {
                    ["nomArchivoReporte"] = request.NomArchivoReporte,
                    ["codTipoArchivoReporte"] = request.CodTipoArchivoReporte.ToString(),
                    ["perTributario"] = request.PerTributario,
                    ["codProceso"] = request.CodProceso,
                    ["numTicket"] = request.NumTicket
                };

                var queryString = string.Join("&", queryParams
                    .Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));

                // 2️⃣ Construcción de URL completa
                var url =
                    $"v1/contribuyente/migeigv/libros/rvierce/gestionprocesosmasivos/web/masivo/archivoreporte?{queryString}";

                using var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                httpRequest.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
                httpRequest.Headers.Accept.ParseAdd("application/json");

                // 3️⃣ Envío de solicitud con cancelación
                using var response = await _httpClient
                    .SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                    .ConfigureAwait(false);

                var statusCode = (int)response.StatusCode;

                // 4️⃣ Manejo de errores HTTP
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

                    string errorMessage;
                    try
                    {
                        var errorObj = JsonConvert.DeserializeObject<SunatErrorResponse>(errorContent);
                        errorMessage = errorObj?.Msg ??
                                       errorObj?.Errors?.FirstOrDefault()?.Msg ??
                                       $"Error {statusCode}: {response.ReasonPhrase}";
                    }
                    catch
                    {
                        errorMessage = $"Error {statusCode}: {response.ReasonPhrase}";
                    }

                    return ResponseFactory.Error<DescargarArchivoReporteResponse>(
                        errorMessage,
                        "SUNAT_ERROR",
                        statusCode
                    );
                }

                // 5️⃣ Leer bytes del archivo
                var archivoBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
                if (archivoBytes == null || archivoBytes.Length == 0)
                {
                    return ResponseFactory.Error<DescargarArchivoReporteResponse>(
                        "La respuesta del servicio no contiene datos.",
                        "EMPTY_RESPONSE",
                        statusCode
                    );
                }

                // 6️⃣ Construir respuesta exitosa
                var data = new DescargarArchivoReporteResponse
                {
                    Archivo = archivoBytes,
                    NombreArchivo = request.NomArchivoReporte
                };

                return ResponseFactory.Success(
                    data,
                    "Archivo descargado correctamente.",
                    statusCode
                );
            }
            catch (HttpRequestException ex)
            {
                return ResponseFactory.Error<DescargarArchivoReporteResponse>(
                    $"Error de red al conectarse con SUNAT: {ex.Message}",
                    "HTTP_REQUEST_EXCEPTION",
                    500
                );
            }
            catch (TaskCanceledException ex)
            {
                return ResponseFactory.Error<DescargarArchivoReporteResponse>(
                    $"Timeout al intentar descargar el archivo: {ex.Message}",
                    "TIMEOUT_EXCEPTION",
                    504
                );
            }
            catch (Exception ex)
            {
                return ResponseFactory.Error<DescargarArchivoReporteResponse>(
                    $"Error inesperado: {ex.Message}",
                    "GENERAL_EXCEPTION",
                    500
                );
            }
        }
    }
}
