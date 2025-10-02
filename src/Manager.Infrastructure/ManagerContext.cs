namespace Manager.Infrastructure
{
    public class ManagerContext : IdentityDbContext<
        User,
        Role,
        Guid,
        IdentityUserClaim<Guid>,
        UserRole, // 👈 tu entidad intermedia personalizada
        IdentityUserLogin<Guid>,
        IdentityRoleClaim<Guid>,
        IdentityUserToken<Guid>
    >, IUnitOfWork
    {
        public const string DEFAULT_SCHEMA = "manager";

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Comprobante> Comprobantes { get; set; }
        public DbSet<ConfiguracionGlobal> ConfiguracionGlobals { get; set; }
        public DbSet<CuentaBaseSOL> CuentaBaseSOLs { get; set; }
        public DbSet<Grupo> Grupos { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Persona> Personas { get; set; }
        public DbSet<PerTributario> PerTributarios { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TokenBase> TokenBases { get; set; }
        public DbSet<Token> Tokens { get; set; }

        public DbSet<User> Users { get; set; }
        //public DbSet<UserRole> UserRoles { get; set; }

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
            modelBuilder.Entity<UserRole>().ToTable("UsuarioRoles"); // 👈 tu tabla intermedi
            modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UsuarioClaims");
            modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UsuarioLogins");
            modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RolClaims");
            modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UsuarioTokens");

            // 4️⃣ Configuración extra de UserRole para evitar cascadas múltiples
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(ur => new { ur.UserId, ur.RoleId });

                entity.HasOne(ur => ur.User)
                      .WithMany(u => u.UserRoles)
                      .HasForeignKey(ur => ur.UserId)
                      .OnDelete(DeleteBehavior.Restrict); // 👈 evita ciclos

                entity.HasOne(ur => ur.Role)
                      .WithMany(r => r.UserRoles)
                      .HasForeignKey(ur => ur.RoleId)
                      .OnDelete(DeleteBehavior.Restrict); // 👈 evita ciclos
            });

            // 5 Aplicar automáticamente todas las configuraciones de IEntityTypeConfiguration
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
