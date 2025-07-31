using Manager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Manager.Infrastructure.SchemaDefinitions
{
    public class ClienteEntitySchemaDefinition : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.ToTable("Clientes", ManagerContext.DEFAULT_SCHEMA);

            builder.HasKey(x => x.Id);

            builder
                .HasOne(c => c.Token)
                .WithOne(t => t.Cliente)
                .HasForeignKey<Token>(t => t.ClienteId);
        }
    }
}