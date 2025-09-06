namespace Manager.Infrastructure.SchemaDefinitions
{
    public class TokenBaseEntitySchemaDefinition : IEntityTypeConfiguration<TokenBase>
    {
        public void Configure(EntityTypeBuilder<TokenBase> builder)
        {
            builder.ToTable("TokenBases", ManagerContext.DEFAULT_SCHEMA);

            builder.HasKey(x => x.Id);
        }
    }
}