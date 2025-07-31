namespace Manager.Domain.Entities
{
    public class Token : EntityBase
    {
        public string AccessToken { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public DateTime FechaExpiracion { get; set; }

        public Guid ClienteId { get; set; }
        public Cliente Cliente { get; set; }
    }
}
