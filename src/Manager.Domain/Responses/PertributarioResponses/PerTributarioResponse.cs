namespace Manager.Domain.Responses.PerTributarioResponses
{
    public class PerTributarioResponse
    {
        public Guid Id { get; set; }
        public bool IsInactive { get; set; }

        public int mes { get; set; }
        public int anio { get; set; }
        public Guid ClienteId { get; set; }
    }
}