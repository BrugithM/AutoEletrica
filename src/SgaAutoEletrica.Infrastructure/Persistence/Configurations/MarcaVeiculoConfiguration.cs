using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SgaAutoEletrica.Domain.Entities;

namespace SgaAutoEletrica.Infrastructure.Persistence.Configurations;

public class MarcaVeiculoConfiguration : IEntityTypeConfiguration<MarcaVeiculo>
{
    public void Configure(EntityTypeBuilder<MarcaVeiculo> builder)
    {
        builder.ToTable("MarcasVeiculo");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Nome).IsRequired().HasMaxLength(100);
        builder.Property(m => m.Ativo).IsRequired();
    }
}