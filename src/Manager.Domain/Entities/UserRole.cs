namespace Manager.Domain.Entities
{
    public class UserRole : IdentityUserRole<Guid>
    {
        // 🔹 Navegación explícita hacia User y Role
        public User User { get; set; }
        public Role Role { get; set; }
    }
}