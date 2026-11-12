using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Infrastructure.SchemaDefinitions
{
    public class SunatPadronHistorialEntitySchemaDefinition : IEntityTypeConfiguration<SunatPadronHistorial>
    {
        public void Configure(EntityTypeBuilder<SunatPadronHistorial> builder)
        {
            builder.ToTable("SunatPadronHistorials");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.NombrePadron)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.TextoFechaSunat)
                .IsRequired()
                .HasMaxLength(250);
        }
    }
}
