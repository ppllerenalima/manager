namespace Manager.Infrastructure.SchemaDefinitions
{
    public class ClienteEntitySchemaDefinition : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.ToTable("Clientes", ManagerContext.DEFAULT_SCHEMA);

            builder.HasKey(x => x.Id);

            // Relación 1:1 Cliente → Token
            builder
                .HasOne(c => c.Token)
                .WithOne(t => t.Cliente)
                .HasForeignKey<Token>(t => t.ClienteId);

            // Relación 1:N Grupo → Clientes
            builder
                .HasOne(c => c.Grupo)
                .WithMany(g => g.Clientes)
                .HasForeignKey(c => c.GrupoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación 1:N User → Clientes
            builder
                .HasOne(c => c.User)
                .WithMany(g => g.Clientes)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}