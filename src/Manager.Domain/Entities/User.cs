namespace Manager.Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        public Guid PersonaId { get; set; }
        public Persona Persona { get; set; }

        // 🔹 Relación con UserRole
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}