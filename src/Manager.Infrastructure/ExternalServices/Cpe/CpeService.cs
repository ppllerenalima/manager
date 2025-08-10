using BillConsultServiceClient;
using Manager.Domain.Requests.Cpe;
using Manager.Domain.Responses.CpeResponses;
using Manager.Domain.Services;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.Text.Json;

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

        public async Task<ConsultaCpeUnificadoResponse> ControlCpeConsultaXmlAsync(string token, ConsultaCpeComprobanteRequest request)
        {
            var responseResult = new ConsultaCpeUnificadoResponse();
            var url = $"v1/contribuyente/controlcpe/consultaxml/{request.RucEmisor}-{request.TipoComprobante}-{request.Serie}-{request.Numero}";

            try
            {
                using var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                using var response = await _httpClient.SendAsync(httpRequest); // 👈 ahora sí se envía el header
                responseResult.StatusCode = (int)response.StatusCode;

                if (response.IsSuccessStatusCode)
                {
                    responseResult.Archivo = await response.Content.ReadAsByteArrayAsync();
                    responseResult.NombreArchivo = $"{request.RucEmisor}-{request.TipoComprobante}-{request.Serie}-{request.Numero}";
                    responseResult.EsExito = true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    responseResult.EsExito = false;
                    responseResult.Errores.AddRange(ProcesarErrorControl(errorContent));
                }
            }
            catch (Exception ex)
            {
                responseResult.EsExito = false;
                responseResult.StatusCode = 500;
                responseResult.Errores.Add(new ErrorConsultaCpeResponse
                {
                    status = "EX",
                    message = ex.Message
                });
            }

            return responseResult;
        }


        public async Task<ConsultaCpeUnificadoResponse> ConsultaCpeComprobanteAsync(string token, ConsultaCpeComprobanteRequest request)
        {
            var responseResult = new ConsultaCpeUnificadoResponse();
            var url = $"v1/contribuyente/consultacpe/comprobantes/{request.RucEmisor}-{request.TipoComprobante}-{request.Serie}-{request.Numero}-3/{request.Tipo}";

            try
            {
                using var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                using var response = await _httpClient.SendAsync(httpRequest);
                responseResult.StatusCode = (int)response.StatusCode;

                var json = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var resultado = JsonSerializer.Deserialize<ConsultaCpeComprobanteResponse>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    responseResult.Archivo = Convert.FromBase64String(resultado.ValArchivo ?? "");
                    responseResult.NombreArchivo = resultado.NomArchivo;
                    responseResult.EsExito = true;
                }
                else
                {
                    responseResult.EsExito = false;
                    responseResult.Errores.AddRange(ProcesarErrorControl(json));
                }
            }
            catch (Exception ex)
            {
                responseResult.EsExito = false;
                responseResult.StatusCode = 500;
                responseResult.Errores.Add(new ErrorConsultaCpeResponse
                {
                    status = "EX",
                    message = ex.Message
                });
            }

            return responseResult;
        }

        private List<ErrorConsultaCpeResponse> ProcesarErrorControl(string json)
        {
            var errores = new List<ErrorConsultaCpeResponse>();

            try
            {
                // 1️⃣ Si es HTML (probable 504)
                if (json.TrimStart().StartsWith("<"))
                {
                    errores.Add(new ErrorConsultaCpeResponse
                    {
                        status = "SUNAT_504",
                        message = "504 Gateway Time-out: La SUNAT no respondió a tiempo."
                    });
                    return errores;
                }

                // 2️⃣ Intentar detectar estructura por palabras clave
                using var doc = JsonDocument.Parse(json);

                // Estructura 422 con "cod" y "errors"
                if (doc.RootElement.TryGetProperty("cod", out var cod))
                {
                    string codigo = cod.GetString();
                    string mensajePrincipal = doc.RootElement.GetProperty("msg").GetString();

                    if (doc.RootElement.TryGetProperty("errors", out var errorsArray) && errorsArray.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var err in errorsArray.EnumerateArray())
                        {
                            errores.Add(new ErrorConsultaCpeResponse
                            {
                                status = $"SUNAT_{codigo}_{err.GetProperty("codError").GetString()}",
                                message = err.GetProperty("desError").GetString()
                            });
                        }
                    }
                    else
                    {
                        errores.Add(new ErrorConsultaCpeResponse
                        {
                            status = $"SUNAT_{codigo}",
                            message = mensajePrincipal
                        });
                    }

                    return errores;
                }

                // Estructura 500 con "code" y "message"
                if (doc.RootElement.TryGetProperty("code", out var code500))
                {
                    errores.Add(new ErrorConsultaCpeResponse
                    {
                        status = $"SUNAT_{code500.GetInt32()}",
                        message = doc.RootElement.GetProperty("message").GetString()
                    });
                    return errores;
                }
            }
            catch
            {
                // Si no se pudo parsear JSON, devolvemos el raw como mensaje
            }

            // 3️⃣ Fallback: respuesta desconocida
            errores.Add(new ErrorConsultaCpeResponse
            {
                status = "SUNAT_ERROR",
                message = json.Length > 500 ? json.Substring(0, 500) + "..." : json
            });

            return errores;
        }

        public async Task<StatusResponse> StatusCdrAsync(ConsultarCpeRequest request)
        {
            var response = new StatusResponse();

            try
            {
                var client = new billServiceClient();

                // Añadimos el comportamiento WS-Security
                client.Endpoint.EndpointBehaviors.Add(
                    new WsSecurityEndpointBehavior($"{request.RucConsulta}{request.Username}", request.Password));

                await client.OpenAsync();

                var result = await client.getStatusAsync(
                    request.RucEmisor,
                    request.TipoComprobante,
                    request.Serie,
                    request.Numero
                );

                response.StatusCode = result.status.statusCode;
                response.StatusMessage = result.status.statusMessage;
                response.Content = result.status.content;
            }
            catch (FaultException faultEx)
            {
                response.Success = false;
                response.ErrorMessage = $"Error SOAP: {faultEx.Message}";
            }
            catch (CommunicationException commEx)
            {
                response.Success = false;
                response.ErrorMessage = $"Error de comunicación: {commEx.Message}";
            }
            catch (TimeoutException timeoutEx)
            {
                response.Success = false;
                response.ErrorMessage = $"Tiempo de espera agotado: {timeoutEx.Message}";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = $"Error general: {ex.Message}";
            }

            return response;
        }
    }
}
