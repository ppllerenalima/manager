namespace Manager.Domain.Responses.ComprobanteResponses
{
    public class Comprobante_GlosaResponse
    {
        public Guid Id { get; set; }
        public string Serie { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string? NombreArchivo { get; set; }
        public string? Glosa { get; set; }
        public DateTime FechaProcesado { get; set; } = DateTime.UtcNow;

        // ✅ Nuevos campos para reportar resultado individual
        public bool Exito { get; set; }
        public string? Mensaje { get; set; }
    }
}
