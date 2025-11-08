namespace Manager.Infrastructure.ExternalServices.ClienteSol.Client
{
    public class ClientesSolClient
    {
        private readonly HttpClient _httpClient;

        public ClientesSolClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<BaseResponseGeneric<SunatAuthResponse>> AccessTokenAsync(
            SunatAuthRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ Construcción de URL
                var url = $"v1/clientessol/{request.ClientId}/oauth2/token/";

                // 2️⃣ Armar el cuerpo del formulario
                var formData = new Dictionary<string, string>
                {
                    ["grant_type"] = request.GrantType,
                    ["scope"] = request.Scope,
                    ["client_id"] = request.ClientId,
                    ["client_secret"] = request.ClientSecret,
                    ["username"] = request.Username,
                    ["password"] = request.Password
                };

                using var content = new FormUrlEncodedContent(formData);
                using var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = content
                };

                // 3️⃣ Enviar solicitud
                using var response = await _httpClient
                    .SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                    .ConfigureAwait(false);

                var statusCode = (int)response.StatusCode;
                var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

                // 4️⃣ Manejar respuesta exitosa
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var result = JsonConvert.DeserializeObject<SunatAuthResponse>(json);
                        return ResponseFactory.Success(result!, "Token obtenido correctamente.", statusCode);
                    }
                    catch (Newtonsoft.Json.JsonException ex)
                    {
                        return ResponseFactory.Error<SunatAuthResponse>(
                            "Error al deserializar la respuesta de autenticación.",
                            "DESERIALIZATION_ERROR",
                            statusCode,
                            ex.Message
                        );
                    }
                }

                // 5️⃣ Manejar errores HTTP
                try
                {
                    var error = JsonConvert.DeserializeObject<ErrorAuthResponse>(json);
                    return ResponseFactory.Error<SunatAuthResponse>(error?.error_description ?? "Error desconocido al autenticar con SUNAT.", error?.error ?? "UNKNOWN", statusCode);
                }
                catch (Newtonsoft.Json.JsonException ex)
                {
                    return ResponseFactory.Error<SunatAuthResponse>(
                        $"Error {statusCode}: no se pudo interpretar la respuesta de SUNAT.",
                        "INVALID_RESPONSE_FORMAT",
                        statusCode,
                        ex.Message
                    );
                }
            }
            catch (HttpRequestException ex)
            {
                return ResponseFactory.Error<SunatAuthResponse>(
                    "Error de red al comunicarse con SUNAT.",
                    "HTTP_REQUEST_EXCEPTION",
                    500,
                    ex.Message
                );
            }
            catch (TaskCanceledException ex)
            {
                return ResponseFactory.Error<SunatAuthResponse>(
                    "Timeout en la autenticación con SUNAT.",
                    "TIMEOUT_EXCEPTION",
                    504,
                    ex.Message
                );
            }
            catch (Exception ex)
            {
                return ResponseFactory.Error<SunatAuthResponse>(
                    "Excepción no controlada.",
                    "GENERAL_EXCEPTION",
                    500,
                    ex.Message
                );
            }
        }
    }
}
