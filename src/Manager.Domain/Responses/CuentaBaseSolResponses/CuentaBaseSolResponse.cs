namespace Manager.Domain.Responses.CuentaBaseSolResponses
{
    public class CuentaBaseSolResponse
    {
        public Guid Id { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}