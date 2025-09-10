using Manager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Manager.Infrastructure.SchemaDefinitions
{
    public class ComprobanteEntitySchemaDefinition : IEntityTypeConfiguration<Comprobante>
    {
        public void Configure(EntityTypeBuilder<Comprobante> builder)
        {
            builder.ToTable("Comprobantes", ManagerContext.DEFAULT_SCHEMA);

            builder.HasKey(x => x.Id);
        }
    }
}