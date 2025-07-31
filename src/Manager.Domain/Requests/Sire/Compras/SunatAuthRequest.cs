namespace Manager.Domain.Requests.Sire.Compras
{
    public class SunatAuthRequest
    {
        public string GrantType { get; set; } = "password"; // por defecto
        public string Scope { get; set; } = "https://api-cpe.sunat.gob.pe";
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
