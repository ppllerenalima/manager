using Manager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Manager.Infrastructure.SchemaDefinitions
{
    public class ComprobanteEntitySchemaDefinition : IEntityTypeConfiguration<Comprobante>
    {
        public void Configure(EntityTypeBuilder<Comprobante> builder)
        {
            builder.ToTable("Comprobantes", ManagerContext.DEFAULT_SCHEMA);

            builder.Property(e => e.Total).HasPrecision(18, 2);

            builder.Property(e => e.IgvDG).HasPrecision(18, 2);

            builder.Property(e => e.TipoCambio).HasPrecision(18, 4);

            builder.HasKey(x => x.Id);
        }
    }
}