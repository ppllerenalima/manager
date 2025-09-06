namespace Manager.Domain.Requests.TokenBase
{
    public class AddTokenBaseRequest
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public int ExpiresIn { get; set; }

        public DateTime FechaGeneracion { get; set; }
        public DateTime FechaExpiracion { get; set; }

        public bool IsInactive { get; set; } = false;

        public Guid CuentaBaseSolId { get; set; }
    }
}