using BillConsultServiceClient;
using Manager.Domain.Requests.Cpe;
using Manager.Domain.Responses;
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

        public async Task<ControlCpeConsultaXmlResponse> ControlCpeConsultaXmlAsync(string token, ConsultaCpeComprobanteRequest request)
        {
            var responseResult = new ControlCpeConsultaXmlResponse();
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
                responseResult.Errores.Add(new Error_ControlCpeConsultaXmlResponse
                {
                    status = "EX",
                    message = ex.Message
                });
            }

            return responseResult;
        }


        public async Task<ConsultaCpeComprobanteResponse> ConsultaCpeComprobanteAsync(string token, ConsultaCpeComprobanteRequest request)
        {
            var responseResult = new ConsultaCpeComprobanteResponse();
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

                    responseResult.ValArchivo = resultado.ValArchivo;
                    responseResult.NomArchivo = resultado.NomArchivo;
                    responseResult.EsExito = true;
                }
                else
                {
                    responseResult.EsExito = false;
                    responseResult.Errores.AddRange(ProcesarErrorConsulta(json));
                }
            }
            catch (Exception ex)
            {
                responseResult.EsExito = false;
                responseResult.StatusCode = 500;
                responseResult.Errores.Add(new Error_ConsultaCpeComprobanteResponse
                {
                    status = "EX",
                    message = ex.Message
                });
            }

            return responseResult;
        }

        private List<Error_ControlCpeConsultaXmlResponse> ProcesarErrorControl(string json)
        {
            try
            {
                var errorSunat = JsonSerializer.Deserialize<Error_ControlCpeConsultaXmlResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return new List<Error_ControlCpeConsultaXmlResponse> { errorSunat };
            }
            catch
            {
                return new List<Error_ControlCpeConsultaXmlResponse>
            {
                new Error_ControlCpeConsultaXmlResponse
                {
                    status = "SUNAT_ERROR",
                    message = json
                }
            };
            }
        }

        private List<Error_ConsultaCpeComprobanteResponse> ProcesarErrorConsulta(string json)
        {
            try
            {
                var errorSunat = JsonSerializer.Deserialize<Error_ConsultaCpeComprobanteResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return new List<Error_ConsultaCpeComprobanteResponse> { errorSunat };
            }
            catch
            {
                return new List<Error_ConsultaCpeComprobanteResponse>
            {
                new Error_ConsultaCpeComprobanteResponse
                {
                    status = "SUNAT_ERROR",
                    message = json
                }
            };
            }
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
