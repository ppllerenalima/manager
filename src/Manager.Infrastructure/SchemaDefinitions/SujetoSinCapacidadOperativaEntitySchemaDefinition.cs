using Manager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Manager.Infrastructure.SchemaDefinitions
{
    public class SujetoSinCapacidadOperativaEntitySchemaDefinition : IEntityTypeConfiguration<SujetoSinCapacidadOperativa>
    {
        public void Configure(EntityTypeBuilder<SujetoSinCapacidadOperativa> builder)
        {
            builder.ToTable("SujetoSinCapacidadOperativas");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Ruc)
                .IsRequired()
                .HasMaxLength(11)
                .IsUnicode(false);

            builder.Property(x => x.RazonSocial)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(x => x.DomicilioFiscal)
                .HasMaxLength(500);

            builder.Property(x => x.NumeroResolucion)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.DocumentoRepresentanteLegal)
                .HasMaxLength(150);

            builder.Property(x => x.NombreRepresentanteLegal)
                .HasMaxLength(300);

            // Índice para cruzar al instante con tus compras del SIRE
            builder.HasIndex(x => x.Ruc);

            // Índice único compuesto para evitar duplicados por RUC + Resolución
            builder.HasIndex(x => new { x.Ruc, x.NumeroResolucion })
                .IsUnique();
        }
    }
}
