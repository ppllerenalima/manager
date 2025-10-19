namespace Manager.Domain.Responses
{
    public class    DescargarArchivoReporteResponse
    {
        public bool EsExito { get; set; }
        public int StatusCode { get; set; }
        public byte[] Archivo { get; set; } // Si quieres devolver el ZIP como byte[]
        public string NombreArchivo { get; set; }

        public List<Error_DescargarArchivoReporteResponse> Errores { get; set; } = new List<Error_DescargarArchivoReporteResponse>();
    }

    public class Error_DescargarArchivoReporteResponse
    {
        public string status { get; set; }
        public string message { get; set; }
    }
}
