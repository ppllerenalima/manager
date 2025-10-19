namespace Manager.Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        public Guid PersonaId { get; set; }
        public Persona Persona { get; set; } = default!;

        // 🔹 Relación con UserRole
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();
    }
}