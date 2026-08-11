using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SgaAutoEletrica.Domain.Entities;

namespace SgaAutoEletrica.Infrastructure.Persistence.Configurations;

public class ItemPecaOSConfiguration : IEntityTypeConfiguration<ItemPecaOS>
{
    public void Configure(EntityTypeBuilder<ItemPecaOS> builder)
    {
        builder.ToTable("ItensPecaOS");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Quantidade).IsRequired();
        builder.Property(i => i.PrecoUnitario).HasColumnType("decimal(10,2)");
        builder.Property(i => i.ValorTotal).HasColumnType("decimal(10,2)");

        builder.HasOne(i => i.Peca)
            .WithMany()
            .HasForeignKey(i => i.PecaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}