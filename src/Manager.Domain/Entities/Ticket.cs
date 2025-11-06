namespace Manager.Domain.Entities
{
    public class Ticket : EntityBase
    {
        public string CodProceso { get; set; }
        public string CodEstadoProceso { get; set; }
        public string DesProceso { get; set; }
        public string PerTributario { get; set; }

        public string NumTicket { get; set; }
        public string FecCargaImportacion { get; set; }
        public string HoraCargaImportacion { get; set; }
        public string CodEstadoEnvio { get; set; }
        public string DesEstadoEnvio { get; set; }

        public string? CodTipoAchivoReporte { get; set; } = null;
        public string? NomArchivoReporte { get; set; } = null;

        public Guid ClienteId { get; set; }
        public Cliente Cliente { get; set; }
    }
}