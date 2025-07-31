namespace Manager.Domain.Responses
{
    //public class SunatAuthResponse
    //{
    //    [JsonPropertyName("access_token")]
    //    public string AccessToken { get; set; }

    //    [JsonPropertyName("token_type")]
    //    public string TokenType { get; set; }

    //    [JsonPropertyName("expires_in")]
    //    public int ExpiresIn { get; set; }
    //}
    public class SunatAuthResponse
    {
        public bool EsExito { get; set; }
        public int StatusCode { get; set; }
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public int ExpiresIn { get; set; }

        public List<ErrorDetail> Errores { get; set; } = new();
    }

    //public class ErrorDetail
    //{
    //    public string Codigo { get; set; }
    //    public string Descripcion { get; set; }
    //}
}
