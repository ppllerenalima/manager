using Manager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Manager.Infrastructure.SchemaDefinitions
{
    public class PertributarioEntitySchemaDefinition : IEntityTypeConfiguration<PerTributario>
    {
        public void Configure(EntityTypeBuilder<PerTributario> builder)
        {
            builder.ToTable("Pertributarios", ManagerContext.DEFAULT_SCHEMA);

            builder.HasKey(x => x.Id);

            builder.HasIndex(p => new { p.ClienteId, p.mes, p.anio, p.TipoComprobante })
                .IsUnique();

            builder
                .HasOne(p => p.Cliente)
                .WithMany(c => c.PeriodosTributarios)
                .HasForeignKey(p => p.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}