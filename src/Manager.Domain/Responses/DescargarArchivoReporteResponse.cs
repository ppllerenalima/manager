namespace Manager.Domain.Responses
{
    public class DescargarArchivoReporteResponse
    {
        public byte[] Archivo { get; set; } // Si quieres devolver el ZIP como byte[]
        public string NombreArchivo { get; set; }
        public string ErrorContent { get; set; }
    }
}
