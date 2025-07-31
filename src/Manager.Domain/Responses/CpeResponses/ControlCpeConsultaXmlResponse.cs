namespace Manager.Domain.Responses.CpeResponses
{
    public class ControlCpeConsultaXmlResponse
    {
        public bool EsExito { get; set; }
        public int StatusCode { get; set; }
        public byte[] Archivo { get; set; } // Si quieres devolver el ZIP como byte[]
        public string NombreArchivo { get; set; }
        public List<Error_ControlCpeConsultaXmlResponse> Errores { get; set; } = new List<Error_ControlCpeConsultaXmlResponse>();
    }

    public class Error_ControlCpeConsultaXmlResponse
    {
        public string status { get; set; }
        public string message { get; set; }
    }
}
