

namespace Manager.Infrastructure.ExternalServices.Sire
{
    public class SireComprasService : ISireComprasService
    {
        private readonly HttpClient _httpClient;
        private readonly IZipFileParser _zipFileParser;

        public SireComprasService(HttpClient httpClient, IZipFileParser zipFileParser)
        {
            _httpClient = httpClient;
            _zipFileParser = zipFileParser;
        }

        public async Task<SunatAuthResponse> AccessTokenAsync(SunatAuthRequest request)
        {
            var formData = new Dictionary<string, string>
            {
                ["grant_type"] = request.GrantType,
                ["scope"] = request.Scope,
                ["client_id"] = request.ClientId,
                ["client_secret"] = request.ClientSecret,
                ["username"] = request.Username,
                ["password"] = request.Password
            };

            var content = new FormUrlEncodedContent(formData);
            var url = $"https://api-seguridad.sunat.gob.pe/v1/clientessol/{request.ClientId}/oauth2/token/";

            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            var result = new SunatAuthResponse
            {
                StatusCode = (int)response.StatusCode,
                EsExito = response.IsSuccessStatusCode
            };

            if (response.IsSuccessStatusCode)
            {
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                result.AccessToken = root.GetProperty("access_token").GetString();
                result.TokenType = root.GetProperty("token_type").GetString();
                result.ExpiresIn = root.GetProperty("expires_in").GetInt32();
            }
            else
            {
                try
                {
                    using var jsonDoc = JsonDocument.Parse(responseContent);
                    var root = jsonDoc.RootElement;

                    result.Errores.Add(new ErrorDetail
                    {
                        Cod = root.GetProperty("error").GetString(),
                        Msg = root.GetProperty("error_descripcion").GetString()
                    });
                }
                catch
                {
                    result.Errores.Add(new ErrorDetail
                    {
                        Cod = "Desconocido",
                        Msg = "No se pudo leer el mensaje de error."
                    });
                }
            }

            return result;
        }

        public async Task<AceptarPropuestaResultado> AceptarPropuestaAsync(AceptarPropuestaRequest requestModel)
        {
            var url = $"https://api-sire.sunat.gob.pe/v1/contribuyente/migeigv/libros/rce/propuesta/web/registroslibros/{requestModel.PeriodoTributario}/aceptarpropuesta";

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", requestModel.AccessToken);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent(string.Empty, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.UnprocessableEntity)
                {
                    var errorObj = JsonConvert.DeserializeObject<SunatErrorResponse>(responseContent);

                    var error42207 = errorObj?.Errors?.FirstOrDefault(e => e.Cod == 42207);
                    if (error42207 != null)
                    {
                        return new AceptarPropuestaResultado
                        {
                            FueAceptada = false,
                            NumTicket = null,
                            Mensaje = "Ya existe un preliminar registrado para este período."
                        };
                    }

                    return new AceptarPropuestaResultado
                    {
                        FueAceptada = false,
                        NumTicket = null,
                        Mensaje = $"Error de validación SUNAT: {errorObj?.Msg}"
                    };
                }

                throw new Exception($"Error al aceptar propuesta: {response.StatusCode} - {responseContent}");
            }

            return new AceptarPropuestaResultado
            {
                FueAceptada = true,
                NumTicket = responseContent.Trim('"'), // Por si viene con comillas
                Mensaje = "Propuesta aceptada correctamente."
            };
        }

        public async Task<DescargarPropuestaResponse> DescargarPropuestaRCEAsync(DescargarPropuestaRequest request)
        {
            var responseResult = new DescargarPropuestaResponse();

            try
            {
                // Validaciones previas
                if (string.IsNullOrWhiteSpace(request.Token)) throw new ArgumentException("El token es obligatorio.");
                if (string.IsNullOrWhiteSpace(request.PerTributario)) throw new ArgumentException("El campo 'perTributario' es obligatorio.");
                if (request.CodTipoArchivo < 0 || request.CodTipoArchivo > 1) throw new ArgumentException("El campo 'codTipoArchivo' debe ser mayor a cero y menor que 1.");
                if (string.IsNullOrWhiteSpace(request.CodOrigenEnvio)) throw new ArgumentException("El campo 'codOrigenEnvio' es obligatorio.");
                if (string.IsNullOrWhiteSpace(request.CodTipoCDP)) throw new ArgumentException("El campo 'CodTipoCDP' es obligatorio.");
                //if (string.IsNullOrWhiteSpace(request.NumSerieCDP)) throw new ArgumentException("El campo 'numSerieCDP' es obligatorio.");
                //if (string.IsNullOrWhiteSpace(request.NumCDP)) throw new ArgumentException("El campo 'numCDP' es obligatorio.");

                if ((request.MtoDesde.HasValue && !request.MtoHasta.HasValue) || (!request.MtoDesde.HasValue && request.MtoHasta.HasValue))
                {
                    throw new ArgumentException("Si se incluye búsqueda por monto, ambos campos 'mtoDesde' y 'mtoHasta' deben tener valor.");
                }

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://api-sire.sunat.gob.pe/");
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", request.Token);

                    var queryParams = new Dictionary<string, string>
                    {
                        ["codTipoArchivo"] = request.CodTipoArchivo.ToString(),
                        ["codOrigenEnvio"] = request.CodOrigenEnvio,
                        ["codTipoCDP"] = request.CodTipoCDP,
                    };

                    if (!string.IsNullOrWhiteSpace(request.FecEmisionIni)) queryParams["fecEmisionIni"] = request.FecEmisionIni;
                    if (!string.IsNullOrWhiteSpace(request.FecEmisionFin)) queryParams["fecEmisionFin"] = request.FecEmisionFin;
                    if (!string.IsNullOrWhiteSpace(request.CodTipoCDP)) queryParams["codTipoCDP"] = request.CodTipoCDP;
                    if (!string.IsNullOrWhiteSpace(request.CodInconsistencia)) queryParams["codInconsistencia"] = request.CodInconsistencia;
                    if (!string.IsNullOrWhiteSpace(request.CodCar)) queryParams["codCar"] = request.CodCar;
                    if (!string.IsNullOrWhiteSpace(request.NumDocAdquiriente)) queryParams["numDocAdquiriente"] = request.NumDocAdquiriente;

                    if (request.MtoDesde.HasValue) queryParams["mtoDesde"] = request.MtoDesde.Value.ToString("F2", CultureInfo.InvariantCulture);
                    if (request.MtoHasta.HasValue) queryParams["mtoHasta"] = request.MtoHasta.Value.ToString("F2", CultureInfo.InvariantCulture);

                    var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
                    var url = $"v1/contribuyente/migeigv/libros/rce/propuesta/web/propuesta/{request.PerTributario}/exportacioncomprobantepropuesta?{queryString}";

                    HttpResponseMessage response = await client.GetAsync(url);
                    responseResult.StatusCode = (int)response.StatusCode;

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var sunatResponse = JsonConvert.DeserializeObject<ResultDescargarPropuesta>(json);

                        responseResult.EsExito = true;
                        responseResult.Result = new ResultDescargarPropuesta { NumTicket = sunatResponse?.NumTicket };
                    }
                    else
                    {
                        var errorJson = await response.Content.ReadAsStringAsync();
                        var errorObj = JsonConvert.DeserializeObject<SunatErrorResponse>(errorJson);

                        responseResult.EsExito = false;

                        if (errorObj?.Errors != null && errorObj.Errors.Count > 0)
                        {
                            responseResult.Errores.AddRange(errorObj.Errors.Select(e => new ErrorDetail
                            {
                                Cod = e.Cod.ToString(),
                                Msg = e.Msg
                            }));
                        }
                        else
                        {
                            responseResult.Errores.Add(new ErrorDetail
                            {
                                Cod = errorObj?.Cod.ToString() ?? "ERROR",
                                Msg = errorObj?.Msg ?? "Ocurrió un error inesperado"
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                responseResult.EsExito = false;
                responseResult.StatusCode = 500;
                responseResult.Errores.Add(new ErrorDetail
                {
                    Cod = "EX",
                    Msg = ex.Message
                });
            }

            return responseResult;
        }

        public async Task<ConsultarEstadoTicketResponse> ConsultarEstadoTicketAsync(ConsultarEstadoTicketRequest request)
        {
            var url = $"https://api-sire.sunat.gob.pe/v1/contribuyente/migeigv/libros/rvierce/gestionprocesosmasivos/web/masivo/consultaestadotickets" +
                      $"?perIni={request.PerIni}&perFin={request.PerFin}&page={request.Page}&perPage={request.PerPage}";

            if (!string.IsNullOrEmpty(request.NumTicket))
                url += $"&numTicket={request.NumTicket}";

            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", request.AccessToken);
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using var response = await _httpClient.SendAsync(httpRequest);
            var json = await response.Content.ReadAsStringAsync();

            var resultado = new ConsultarEstadoTicketResponse
            {
                StatusCode = (int)response.StatusCode,
                Errores = new List<ErrorDetail>()
            };

            if (response.IsSuccessStatusCode)
            {
                resultado.EsExito = true;
                resultado.Result = JsonConvert.DeserializeObject<Result>(json);
                return resultado;
            }

            // Si es error 422, intenta deserializar los errores específicos
            if ((int)response.StatusCode == 422)
            {
                try
                {
                    var errorResponse = JsonConvert.DeserializeObject<Error422Response>(json);
                    resultado.Errores = errorResponse?.Errors ?? new List<ErrorDetail>();
                }
                catch
                {
                    resultado.Errores.Add(new ErrorDetail
                    {
                        Cod = "Deserialización",
                        Msg = "No se pudo interpretar el cuerpo del error 422."
                    });
                }
            }
            else
            {
                resultado.Errores.Add(new ErrorDetail
                {
                    Cod = response.StatusCode.ToString(),
                    Msg = response.ReasonPhrase
                });
            }

            resultado.EsExito = false;
            return resultado;
        }

        public async Task<DescargarArchivoReporteResponse> DescargarArchivoReporteAsync(string token, DescargarArchivoReporteRequest request)
        {
            var responseResult = new DescargarArchivoReporteResponse();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://api-sire.sunat.gob.pe/");
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var queryString = $"nomArchivoReporte={Uri.EscapeDataString(request.NomArchivoReporte)}" +
                                      $"&codTipoArchivoReporte={request.CodTipoArchivoReporte}" +
                                      //$"&codLibro={request.CodLibro}" +
                                      $"&perTributario={request.PerTributario}" +
                                      $"&codProceso={request.CodProceso}" +
                                      $"&numTicket={request.NumTicket}";

                    var url = $"v1/contribuyente/migeigv/libros/rvierce/gestionprocesosmasivos/web/masivo/archivoreporte?{queryString}";

                    var response = await client.GetAsync(url);
                    responseResult.StatusCode = (int)response.StatusCode;

                    if (response.IsSuccessStatusCode)
                    {
                        responseResult.Archivo = await response.Content.ReadAsByteArrayAsync();
                        responseResult.NombreArchivo = request.NomArchivoReporte;
                        responseResult.EsExito = true;
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        responseResult.Errores.Add(new Error_DescargarArchivoReporteResponse
                        {
                            status = "SUNAT_ERROR",
                            message = errorContent
                        });
                        responseResult.EsExito = false;
                    }
                }
            }
            catch (Exception ex)
            {
                responseResult.EsExito = false;
                responseResult.StatusCode = 500;
                responseResult.Errores.Add(new Error_DescargarArchivoReporteResponse
                {
                    status = "EX",
                    message = ex.Message
                });
            }

            return responseResult;
        }
    }
}
