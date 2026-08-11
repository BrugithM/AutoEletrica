using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SgaAutoEletrica.Domain.Entities;

namespace SgaAutoEletrica.Infrastructure.Persistence.Configurations;

public class CategoriaPecaConfiguration : IEntityTypeConfiguration<CategoriaPeca>
{
    public void Configure(EntityTypeBuilder<CategoriaPeca> builder)
    {
        builder.ToTable("CategoriasPecas");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nome).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Descricao).HasMaxLength(300);
    }
}