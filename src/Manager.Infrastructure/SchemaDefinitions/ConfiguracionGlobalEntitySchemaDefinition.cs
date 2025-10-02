namespace Manager.Infrastructure.SchemaDefinitions
{
    public class ConfiguracionGlobalEntitySchemaDefinition : IEntityTypeConfiguration<ConfiguracionGlobal>
    {
        public void Configure(EntityTypeBuilder<ConfiguracionGlobal> builder)
        {
            builder.ToTable("ConfiguracionGlobals", ManagerContext.DEFAULT_SCHEMA);

            builder.HasKey(c => c.Id);

            builder.Property(c => c.IsInactive)
                .HasDefaultValue(true);
        }
    }
}
