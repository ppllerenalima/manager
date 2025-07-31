using Manager.Domain.Entities;
using Manager.Domain.Repositories;
using Manager.Infrastructure.SchemaDefinitions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Manager.Infrastructure
{
    public class ManagerContext : IdentityDbContext<User>, IUnitOfWork
    {
        public const string DEFAULT_SCHEMA = "manager";
        public DbSet<Cliente> Clientesunats { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        public ManagerContext(DbContextOptions<ManagerContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ClienteEntitySchemaDefinition());
            modelBuilder.ApplyConfiguration(new TokenEntitySchemaDefinition());
            modelBuilder.ApplyConfiguration(new TicketEntitySchemaDefinition());

            base.OnModelCreating(modelBuilder);
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            await SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}