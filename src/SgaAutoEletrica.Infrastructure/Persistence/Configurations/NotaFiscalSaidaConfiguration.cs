using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SgaAutoEletrica.Domain.Entities;

namespace SgaAutoEletrica.Infrastructure.Persistence.Configurations;

public class NotaFiscalSaidaConfiguration : IEntityTypeConfiguration<NotaFiscalSaida>
{
    public void Configure(EntityTypeBuilder<NotaFiscalSaida> builder)
    {
        builder.ToTable("NotasFiscaisSaida");
        builder.HasKey(nf => nf.Id);

        builder.Property(nf => nf.Numero).IsRequired().HasMaxLength(50);
        builder.Property(nf => nf.DataEmissao).IsRequired();
        builder.Property(nf => nf.Observacao).HasMaxLength(500);
        builder.Property(nf => nf.ValorTotal).HasColumnType("decimal(10,2)");

        builder.HasOne(nf => nf.Cliente)
            .WithMany()
            .HasForeignKey(nf => nf.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(nf => nf.Veiculo)
            .WithMany()
            .HasForeignKey(nf => nf.VeiculoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(nf => nf.OrdemServico)
            .WithMany()
            .HasForeignKey(nf => nf.OrdemServicoId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(nf => nf.Itens)
            .WithOne(i => i.NotaFiscalSaida)
            .HasForeignKey(i => i.NotaFiscalSaidaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}