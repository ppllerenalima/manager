namespace Manager.Domain.Responses.TokenResponses
{
    public class TokenResponse
    {
        public Guid Id { get; set; }
        public string? AccessToken { get; set; }
        public DateTime? FechaGeneracion { get; set; }
        public DateTime? FechaExpiracion { get; set; }
        public bool IsInactive { get; set; }
        public string UserId { get; set; }

        // 👇 Información básica del usuario
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? FullName { get; set; }
        public string? Role { get; set; }
    }
}