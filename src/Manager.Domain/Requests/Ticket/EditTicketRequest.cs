namespace Manager.Domain.Requests.Ticket
{
    public class EditTicketRequest
    {
        public Guid Id { get; set; }
        public bool IsInactive { get; set; } = false;

        public string CodProceso { get; set; }
        public string CodEstadoProceso { get; set; }
        public string DesProceso { get; set; }
        public string PerTributario { get; set; }

        public string NumTicket { get; set; }
        public string FecCargaImportacion { get; set; }
        public string HoraCargaImportacion { get; set; }
        public string CodEstadoEnvio { get; set; }
        public string DesEstadoEnvio { get; set; }

        public string CodTipoAchivoReporte { get; set; }
        public string NomArchivoReporte { get; set; }

        public Guid ClienteId { get; set; }
    }
}