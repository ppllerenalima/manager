namespace Manager.Domain.Requests.CuentaBaseSol
{
    public class EditCuentaBaseSolRequest
    {
        public Guid Id { get; set; }
        public bool IsInactive { get; set; } = false;

        public string ClientSecret { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}