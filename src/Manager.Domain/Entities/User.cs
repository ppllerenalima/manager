namespace Manager.Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        public Guid PersonaId { get; set; }
        public Persona Persona { get; set; }
    }
}