namespace Manager.Domain.Requests.Comprobante
{
    public class Comprobante_ImportarGlosaRequest
    {
        public Guid PerTributarioId { get; set; }
        public Guid ClienteId { get; set; }
    }
}
