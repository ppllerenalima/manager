namespace Manager.Domain.Entities
{
    public class User : IdentityUser
    {
        public Guid PersonaId { get; set; }
        public Persona Persona { get; set; }
    }
}