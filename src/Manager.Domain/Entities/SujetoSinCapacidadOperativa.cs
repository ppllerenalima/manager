namespace Manager.Domain.Entities
{
    public class SujetoSinCapacidadOperativa : EntityBase
    {
        public string Ruc { get; set; } = string.Empty;
        public string RazonSocial { get; set; } = string.Empty;
        public string? DomicilioFiscal { get; set; }
        public string NumeroResolucion { get; set; } = string.Empty;
        public DateOnly? FechaEmisionResolucion { get; set; }
        public DateOnly? FechaFirmezaResolucion { get; set; }
        public string? DocumentoRepresentanteLegal { get; set; }
        public string? NombreRepresentanteLegal { get; set; }
        public DateOnly? FechaPublicacion { get; set; }
        public DateTime FechaActualizacionSistema { get; set; } = DateTime.Now;
    }
}
