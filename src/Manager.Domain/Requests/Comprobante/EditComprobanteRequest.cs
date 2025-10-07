namespace Manager.Domain.Requests.Comprobante
{
    public class EditComprobanteRequest
    {
        public Guid Id { get; set; }
        public string Glosa { get; set; }
        public bool TieneGlosa { get; set; } = true;
    }
}
