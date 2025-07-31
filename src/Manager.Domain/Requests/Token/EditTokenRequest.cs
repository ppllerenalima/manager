namespace Manager.Domain.Requests.Token
{
    public class EditTokenRequest
    {
        public Guid Id { get; set; }
        public bool IsInactive { get; set; } = false;

        public string AccessToken { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public DateTime FechaExpiracion { get; set; }

        public Guid ClienteId { get; set; }

    }
}