using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SgaAutoEletrica.Domain.Entities;

namespace SgaAutoEletrica.Infrastructure.Persistence.Configurations;

public class ItemNotaFiscalEntradaConfiguration : IEntityTypeConfiguration<ItemNotaFiscalEntrada>
{
    public void Configure(EntityTypeBuilder<ItemNotaFiscalEntrada> builder)
    {
        builder.ToTable("ItensNotaFiscalEntrada");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Quantidade).IsRequired();
        builder.Property(i => i.ValorUnitario).HasColumnType("decimal(10,2)");
        builder.Property(i => i.ValorTotal).HasColumnType("decimal(10,2)");

        builder.HasOne(i => i.Peca)
            .WithMany()
            .HasForeignKey(i => i.PecaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}