using Manager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Manager.Infrastructure.SchemaDefinitions
{
    public class TokenEntitySchemaDefinition : IEntityTypeConfiguration<Token>
    {
        public void Configure(EntityTypeBuilder<Token> builder)
        {
            builder.ToTable("Tokens", ManagerContext.DEFAULT_SCHEMA);

            builder.HasKey(x => x.Id);
        }
    }
}