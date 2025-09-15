namespace Manager.Domain.Responses.CpeResponses
{
    public class DescargarZipResponse
    {
        public bool EsExito { get; set; }
        public int StatusCode { get; set; }

        // Archivo final (puede venir de cualquiera de los dos servicios)
        public byte[] Archivo { get; set; }
        public string NombreArchivo { get; set; }

        // Errores unificados
        public List<ErrorDescargarZipResponse> Errores { get; set; } = new List<ErrorDescargarZipResponse>();
    }
    public class ErrorDescargarZipResponse
    {
        public string status { get; set; }
        public string message { get; set; }
    }
}
