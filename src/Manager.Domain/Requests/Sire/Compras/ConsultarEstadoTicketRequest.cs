namespace Manager.Domain.Requests.Sire.Compras
{
    public class ConsultarEstadoTicketRequest
    {
        public string AccessToken { get; set; }
        public string PerIni { get; set; }
        public string PerFin { get; set; }
        public int Page { get; set; }
        public int PerPage { get; set; }
        public string? NumTicket { get; set; } // opcional
    }
}
