namespace Manager.Domain.Requests.Ticket
{
    public class GetTicketRequest
    {
        public Guid clienteId { get; set; }
        public string codProceso { get; set; } = "10";
        public string perTributario { get; set; }

        public int Page { get; set; } = 1;
        public int PerPage { get; set; } = 50;
        public string? NumTicket { get; set; } // opcional
    }
}