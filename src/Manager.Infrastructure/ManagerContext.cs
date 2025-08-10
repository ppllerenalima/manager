namespace Manager.Infrastructure
{
    public class ManagerContext : IdentityDbContext<User>, IUnitOfWork
    {
        public const string DEFAULT_SCHEMA = "manager";

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<CuentaBaseSOL> CuentaBaseSOL { get; set; }
        public DbSet<Grupo> Grupos { get; set; }
        public DbSet<Persona> Personas { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<User> Users { get; set; }

        public ManagerContext(DbContextOptions<ManagerContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1️⃣ Configurar Identity primero
            base.OnModelCreating(modelBuilder);

            // 2️⃣ Usar el esquema por defecto para TODAS las tablas
            modelBuilder.HasDefaultSchema(DEFAULT_SCHEMA);

            // 3️⃣ Renombrar tablas de Identity (opcional pero recomendado)
            modelBuilder.Entity<User>().ToTable("Usuarios");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UsuarioRoles");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UsuarioClaims");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UsuarioLogins");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RolClaims");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UsuarioTokens");

            // 4️⃣ Aplicar automáticamente todas las configuraciones de IEntityTypeConfiguration
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ManagerContext).Assembly);
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            await SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
