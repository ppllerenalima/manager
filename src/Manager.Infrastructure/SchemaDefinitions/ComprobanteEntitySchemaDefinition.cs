namespace Manager.Infrastructure.SchemaDefinitions
{
    public class ComprobanteEntitySchemaDefinition : IEntityTypeConfiguration<Comprobante>
    {
        public void Configure(EntityTypeBuilder<Comprobante> builder)
        {
            builder.ToTable("Comprobantes", ManagerContext.DEFAULT_SCHEMA);

            builder.Property(e => e.BiGravadoDG).HasPrecision(12, 2);

            builder.Property(e => e.IgvDG).HasPrecision(12, 2);

            builder.Property(e => e.BiGravadoDGNG).HasPrecision(12, 2);

            builder.Property(e => e.IgvDGNG).HasPrecision(12, 2);

            builder.Property(e => e.BiGravadoDNG).HasPrecision(12, 2);

            builder.Property(e => e.IgvDNG).HasPrecision(12, 2);

            builder.Property(e => e.ValorAdqNG).HasPrecision(12, 2);

            builder.Property(e => e.Isc).HasPrecision(12, 2);

            builder.Property(e => e.Icbper).HasPrecision(12, 2);

            builder.Property(e => e.OtrosTributos).HasPrecision(12, 2);

            builder.Property(e => e.Total).HasPrecision(12, 4);

            builder.Property(e => e.TipoCambio).HasPrecision(12, 2);

            builder.Property(e => e.PorcPart).HasPrecision(12, 2);

            builder.Property(e => e.Imb).HasPrecision(12, 2);

            builder.HasKey(x => x.Id);
        }
    }
}