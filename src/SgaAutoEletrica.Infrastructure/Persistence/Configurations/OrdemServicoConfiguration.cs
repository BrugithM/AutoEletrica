using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SgaAutoEletrica.Domain.Entities;

namespace SgaAutoEletrica.Infrastructure.Persistence.Configurations;

public class OrdemServicoConfiguration : IEntityTypeConfiguration<OrdemServico>
{
    public void Configure(EntityTypeBuilder<OrdemServico> builder)
    {
        builder.ToTable("OrdensServico");
        builder.HasKey(os => os.Id);

        builder.Property(os => os.Numero).IsRequired();
        builder.Property(os => os.Observacao).HasMaxLength(1000);
        builder.Property(os => os.DataAbertura).IsRequired();

        builder.Property(os => os.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(os => os.ValorTotalPecas).HasColumnType("decimal(10,2)");
        builder.Property(os => os.ValorTotalServicos).HasColumnType("decimal(10,2)");
        builder.Property(os => os.ValorTotal).HasColumnType("decimal(10,2)");

        builder.HasOne(os => os.Cliente)
            .WithMany()
            .HasForeignKey(os => os.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(os => os.Veiculo)
            .WithMany()
            .HasForeignKey(os => os.VeiculoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(os => os.ItensPeca)
            .WithOne(i => i.OrdemServico)
            .HasForeignKey(i => i.OrdemServicoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(os => os.ItensServico)
            .WithOne(i => i.OrdemServico)
            .HasForeignKey(i => i.OrdemServicoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(os => os.Numero).IsUnique();
    }
}