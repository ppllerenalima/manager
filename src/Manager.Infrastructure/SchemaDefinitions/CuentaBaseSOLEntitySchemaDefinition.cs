namespace Manager.Infrastructure.SchemaDefinitions
{
    public class CuentaBaseSOLEntitySchemaDefinition : IEntityTypeConfiguration<CuentaBaseSOL>
    {
        public void Configure(EntityTypeBuilder<CuentaBaseSOL> builder)
        {
            builder.ToTable("CuentaBaseSOLs", ManagerContext.DEFAULT_SCHEMA);

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

            // Relación 1:1 Cliente → Token
            builder
                .HasOne(c => c.TokenBase)
                .WithOne(t => t.CuentaBaseSol)
                .HasForeignKey<TokenBase>(t => t.CuentaBaseSolId);
        }
    }
}
