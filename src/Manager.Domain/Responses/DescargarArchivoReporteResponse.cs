namespace Manager.Domain.Responses
{
    public class DescargarArchivoReporteResponse
    {
        public bool EsExito { get; set; }
        public int StatusCode { get; set; }
        public byte[] Archivo { get; set; } // Si quieres devolver el ZIP como byte[]
        public string NombreArchivo { get; set; }
        public List<ErrorDetail> Errores { get; set; } = new List<ErrorDetail>();
    }
}
