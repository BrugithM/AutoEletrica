using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SgaAutoEletrica.Domain.Entities;

namespace SgaAutoEletrica.Infrastructure.Persistence.Configurations;

public class ItemServicoOSConfiguration : IEntityTypeConfiguration<ItemServicoOS>
{
    public void Configure(EntityTypeBuilder<ItemServicoOS> builder)
    {
        builder.ToTable("ItensServicoOS");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.PrecoUnitario).HasColumnType("decimal(10,2)");

        builder.HasOne(i => i.Servico)
            .WithMany()
            .HasForeignKey(i => i.ServicoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}