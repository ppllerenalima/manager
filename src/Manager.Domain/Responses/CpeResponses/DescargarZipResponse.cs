namespace Manager.Domain.Responses.CpeResponses
{
    public class DescargarZipResponse
    {
        public string? Tipo { get; set; }
        public byte[] Archivo { get; set; }
        public string NombreArchivo { get; set; }
    }
}
