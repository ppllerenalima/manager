namespace Manager.Infrastructure.SchemaDefinitions
{
    public class CuentaBaseSOLEntitySchemaDefinition : IEntityTypeConfiguration<CuentaBaseSOL>
    {
        public void Configure(EntityTypeBuilder<CuentaBaseSOL> builder)
        {
            builder.ToTable("CuentaBaseSOL", ManagerContext.DEFAULT_SCHEMA);

            builder.HasKey(c => c.Id);

            builder.Property(c => c.ClientId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.ClientSecret)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Username)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Password)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.IsInactive)
                .HasDefaultValue(true);
        }
    }
}
