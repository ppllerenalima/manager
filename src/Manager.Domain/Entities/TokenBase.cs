namespace Manager.Domain.Entities
{
    public class TokenBase : EntityBase
    {
        public string AccessToken { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public DateTime FechaExpiracion { get; set; }

        public Guid CuentaBaseSolId { get; set; }
        public CuentaBaseSOL CuentaBaseSol { get; set; }
    }
}
