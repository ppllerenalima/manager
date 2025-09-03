namespace Manager.Domain.Requests.CuentaBaseSol
{
    public class AddCuentaBaseSolRequest
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsInactive { get; set; } = false;
    }
}