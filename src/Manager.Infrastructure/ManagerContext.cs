namespace Manager.Infrastructure
{
    public class ManagerContext : IdentityDbContext<User, Role, Guid>, IUnitOfWork
    {
        public const string DEFAULT_SCHEMA = "manager";

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<CuentaBaseSOL> CuentaBaseSOL { get; set; }
        public DbSet<Grupo> Grupos { get; set; }
        public DbSet<Role> Roles { get; set; }
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
            modelBuilder.Entity<Role>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("UsuarioRoles");
            modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UsuarioClaims");
            modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UsuarioLogins");
            modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RolClaims");
            modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UsuarioTokens");

            // 4️⃣ Aplicar automáticamente todas las configuraciones de IEntityTypeConfiguration
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ManagerContext).Assembly);
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            await SaveChangesAsync(cancellationToken);
            return true;
        }

        // 🚀 Implementación de transacciones
        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            return await Database.BeginTransactionAsync(cancellationToken);
        }
    }
}
