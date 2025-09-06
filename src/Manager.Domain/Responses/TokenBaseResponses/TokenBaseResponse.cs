namespace Manager.Domain.Responses.TokenBaseResponses
{
    public class TokenBaseResponse
    {
        public Guid Id { get; set; }
        public string? AccessToken { get; set; }
        public DateTime? FechaGeneracion { get; set; }
        public DateTime? FechaExpiracion { get; set; }
        public bool IsInactive { get; set; }
        public Guid CuentaBaseSolId { get; set; }
    }
}