namespace Manager.Domain.Requests.Ticket
{
    public class AddTicketRequest
    {
        public bool IsInactive { get; set; } = false;

        public string CodProceso { get; set; }
        public string CodEstadoProceso { get; set; }
        public string DesProceso { get; set; }
        public string PerTributario { get; set; }

        public string NumTicket { get; set; }
        public string FecCargaImportacion { get; set; } = string.Empty;
        public string HoraCargaImportacion { get; set; } = string.Empty;
        public string CodEstadoEnvio { get; set; } = string.Empty;
        public string DesEstadoEnvio { get; set; } = string.Empty;

        public string CodTipoAchivoReporte { get; set; } = string.Empty;
        public string NomArchivoReporte { get; set; } = string.Empty;

        public Guid ClienteId { get; set; }
    }
}