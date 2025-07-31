using Manager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Manager.Infrastructure.SchemaDefinitions
{
    public class TicketEntitySchemaDefinition : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.ToTable("Tickets", ManagerContext.DEFAULT_SCHEMA);

            builder.HasKey(x => x.Id);
        }
    }
}