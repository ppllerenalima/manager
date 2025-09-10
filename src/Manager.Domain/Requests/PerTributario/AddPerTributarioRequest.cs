namespace Manager.Domain.Requests.PerTributario
{
    public class AddPerTributarioRequest
    {
        public bool IsInactive { get; set; } = false;

        public int mes { get; set; }
        public int anio { get; set; }
        public TipoComprobanteEnum TipoComprobante { get; set; }
        public Guid ClienteId { get; set; }   // FK explícita

        public byte[] archivoZip { get; set; }
    }
}