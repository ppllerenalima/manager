namespace Manager.Infrastructure.SchemaDefinitions
{
    public class PersonaEntitySchemaDefinition : IEntityTypeConfiguration<Persona>
    {
        public void Configure(EntityTypeBuilder<Persona> builder)
        {
            builder.ToTable("Personas", ManagerContext.DEFAULT_SCHEMA);

            builder.HasKey(p => p.Id);

            // Relación 1:1 Persona ↔ User
            builder
                .HasOne(p => p.User)
                .WithOne(u => u.Persona)
                .HasForeignKey<User>(u => u.PersonaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}