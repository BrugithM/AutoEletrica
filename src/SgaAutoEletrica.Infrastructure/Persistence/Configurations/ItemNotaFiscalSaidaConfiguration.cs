using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SgaAutoEletrica.Domain.Entities;

namespace SgaAutoEletrica.Infrastructure.Persistence.Configurations;

public class ItemNotaFiscalSaidaConfiguration : IEntityTypeConfiguration<ItemNotaFiscalSaida>
{
    public void Configure(EntityTypeBuilder<ItemNotaFiscalSaida> builder)
    {
        builder.ToTable("ItensNotaFiscalSaida");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Descricao).IsRequired().HasMaxLength(300);
        builder.Property(i => i.Quantidade).IsRequired();
        builder.Property(i => i.ValorUnitario).HasColumnType("decimal(10,2)");
        builder.Property(i => i.ValorTotal).HasColumnType("decimal(10,2)");
    }
}