using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SgaAutoEletrica.Domain.Entities;

namespace SgaAutoEletrica.Infrastructure.Persistence.Configurations;

public class NotaFiscalEntradaConfiguration : IEntityTypeConfiguration<NotaFiscalEntrada>
{
    public void Configure(EntityTypeBuilder<NotaFiscalEntrada> builder)
    {
        builder.ToTable("NotasFiscaisEntrada");
        builder.HasKey(nf => nf.Id);

        builder.Property(nf => nf.Numero).IsRequired().HasMaxLength(50);
        builder.Property(nf => nf.DataEntrada).IsRequired();
        builder.Property(nf => nf.Observacao).HasMaxLength(500);
        builder.Property(nf => nf.ValorTotal).HasColumnType("decimal(10,2)");
        builder.Property(nf => nf.Finalizada).IsRequired();

        builder.HasOne(nf => nf.Fornecedor)
            .WithMany()
            .HasForeignKey(nf => nf.FornecedorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(nf => nf.Itens)
            .WithOne(i => i.NotaFiscalEntrada)
            .HasForeignKey(i => i.NotaFiscalEntradaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}