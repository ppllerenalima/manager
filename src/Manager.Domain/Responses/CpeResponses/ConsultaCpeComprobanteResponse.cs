namespace Manager.Domain.Responses.CpeResponses
{
    public class ConsultaCpeComprobanteResponse
    {
        public bool EsExito { get; set; }
        public int StatusCode { get; set; }

        public string NomArchivo { get; set; }
        public string ValArchivo { get; set; }
        public List<Error_ConsultaCpeComprobanteResponse> Errores { get; set; } = new List<Error_ConsultaCpeComprobanteResponse>();

    }

    public class Error_ConsultaCpeComprobanteResponse
    {
        public string status { get; set; }
        public string message { get; set; }
    }
}
