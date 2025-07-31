namespace Manager.Domain.Responses.CpeResponses
{
    public class ConsultaCpeUnificadoResponse
    {
        public bool EsExito { get; set; }
        public int StatusCode { get; set; }

        // Archivo final (puede venir de cualquiera de los dos servicios)
        public byte[] Archivo { get; set; }
        public string NombreArchivo { get; set; }

        // Errores unificados
        public List<ErrorConsultaCpeResponse> Errores { get; set; } = new List<ErrorConsultaCpeResponse>();
    }
    public class ErrorConsultaCpeResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
    }
}
