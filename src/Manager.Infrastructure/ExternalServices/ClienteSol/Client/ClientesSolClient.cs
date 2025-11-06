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
                        using var jsonDoc = JsonDocument.Parse(json);
                        var root = jsonDoc.RootElement;

                        var result = new SunatAuthResponse
                        {
                            AccessToken = root.GetProperty("access_token").GetString(),
                            TokenType = root.GetProperty("token_type").GetString(),
                            ExpiresIn = root.GetProperty("expires_in").GetInt32(),
                            StatusCode = statusCode,
                            EsExito = true
                        };

                        return ResponseFactory.Success(result, "Token obtenido correctamente.", statusCode);
                    }
                    catch (System.Text.Json.JsonException ex)
                    {
                        return ResponseFactory.Error<SunatAuthResponse>(
                            $"Error al deserializar la respuesta de autenticación: {ex.Message}",
                            "DESERIALIZATION_ERROR",
                            statusCode
                        );
                    }
                }

                // 5️⃣ Manejar errores HTTP
                try
                {
                    using var jsonDoc = JsonDocument.Parse(json);
                    var root = jsonDoc.RootElement;

                    var error = new ErrorDetail
                    {
                        Cod = root.TryGetProperty("error", out var codeProp) ? codeProp.GetString() ?? "UNKNOWN" : "UNKNOWN",
                        Msg = root.TryGetProperty("error_description", out var msgProp)
                            ? msgProp.GetString() ?? "Error desconocido al autenticar con SUNAT."
                            : "Error desconocido al autenticar con SUNAT."
                    };

                    var errorResult = new SunatAuthResponse
                    {
                        EsExito = false,
                        StatusCode = statusCode,
                        Errores = new List<ErrorDetail> { error }
                    };

                    return ResponseFactory.Error<SunatAuthResponse>(error.Msg, error.Cod, statusCode);
                }
                catch
                {
                    return ResponseFactory.Error<SunatAuthResponse>(
                        $"Error {statusCode}: no se pudo interpretar la respuesta de SUNAT.",
                        "INVALID_RESPONSE_FORMAT",
                        statusCode
                    );
                }
            }
            catch (HttpRequestException ex)
            {
                return ResponseFactory.Error<SunatAuthResponse>(
                    $"Error de red al comunicarse con SUNAT: {ex.Message}",
                    "HTTP_REQUEST_EXCEPTION",
                    500
                );
            }
            catch (TaskCanceledException ex)
            {
                return ResponseFactory.Error<SunatAuthResponse>(
                    $"Timeout en la autenticación con SUNAT: {ex.Message}",
                    "TIMEOUT_EXCEPTION",
                    504
                );
            }
            catch (Exception ex)
            {
                return ResponseFactory.Error<SunatAuthResponse>(
                    $"Excepción no controlada: {ex.Message}",
                    "GENERAL_EXCEPTION",
                    500
                );
            }
        }
    }
}
