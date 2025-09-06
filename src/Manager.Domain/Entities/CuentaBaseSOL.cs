namespace Manager.Domain.Entities
{
    public class CuentaBaseSOL : EntityBase
    {
        public string Ruc { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public TokenBase TokenBase { get; set; }
    }
}
