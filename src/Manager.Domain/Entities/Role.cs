namespace Manager.Domain.Entities
{
    public class Role : IdentityRole<Guid>
    {
        // Aquí puedes agregar propiedades personalizadas para el rol
        //public string Descripcion { get; set; }

        // 🔹 Relación con UserRole
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
