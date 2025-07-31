namespace Manager.Domain.Responses.TokenResponses
{
    public class TokenResponse
    {
        public Guid Id { get; set; }
        public string? AccessToken { get; set; }
        public DateTime? FechaGeneracion { get; set; }
        public DateTime? FechaExpiracion { get; set; }

        public bool IsInactive { get; set; }

        public Guid ClienteId { get; set; }
    }
}