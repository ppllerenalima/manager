namespace Manager.Infrastructure.ExternalServices.Cpe.Client
{
    public class ControlAccesoClient
    {
        private readonly HttpClient _httpClient;

        public ControlAccesoClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<BaseResponseGeneric<string>> ActualizarPermisosAsync(string token, ActualizarPermisosRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var url = "v1/tecnologia/controlacceso/aplicaciones";

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

                using var response = await _httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
                var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

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
    }
}
