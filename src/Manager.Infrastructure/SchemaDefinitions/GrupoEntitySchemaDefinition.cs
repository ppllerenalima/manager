namespace Manager.Infrastructure.SchemaDefinitions
{
    public class GrupoEntitySchemaDefinition : IEntityTypeConfiguration<Grupo>
    {
        public void Configure(EntityTypeBuilder<Grupo> builder)
        {
            builder.ToTable("Grupos", ManagerContext.DEFAULT_SCHEMA);

            builder.HasKey(c => c.Id);

            builder.Property(c => c.IsInactive)
                .HasDefaultValue(true);
        }
    }
}
