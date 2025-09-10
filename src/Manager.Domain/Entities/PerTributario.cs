namespace Manager.Domain.Entities
{
    public class PerTributario : EntityBase
    {
        public int mes { get; set; }
        public int anio { get; set; }
        public TipoComprobanteEnum TipoComprobante { get; set; }

        public Guid ClienteId { get; set; }   // FK explícita
        public Cliente Cliente { get; set; }  // Prop de navegación

        // 🔹 Relación con Comprobantes
        public ICollection<Comprobante> Comprobantes { get; set; } = new List<Comprobante>();
    }
}
