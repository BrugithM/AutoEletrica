using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SgaAutoEletrica.Domain.Entities;

namespace SgaAutoEletrica.Infrastructure.Persistence.Configurations;

public class PecaConfiguration : IEntityTypeConfiguration<Peca>
{
    public void Configure(EntityTypeBuilder<Peca> builder)
    {
        builder.ToTable("Pecas");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.IdPeca).IsRequired().HasMaxLength(50);
        builder.Property(p => p.CodigoPeca).HasMaxLength(50);
        builder.Property(p => p.Nome).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Descricao).IsRequired().HasMaxLength(500);
        builder.Property(p => p.Marca).IsRequired().HasMaxLength(100);

        builder.Property(p => p.ValorCusto).HasColumnType("decimal(10,2)");
        builder.Property(p => p.ValorVenda).HasColumnType("decimal(10,2)");
        builder.Property(p => p.Imposto).HasColumnType("decimal(5,2)");

        builder.Property(p => p.Estoque).IsRequired();
        builder.Property(p => p.EstoqueMinimo).IsRequired();
        builder.Property(p => p.Ativo).IsRequired();
        builder.Property(p => p.DataCadastro).IsRequired();

        builder.OwnsOne(p => p.CodigoBarras, cb =>
        {
            cb.Property(c => c.Valor).HasColumnName("CodigoBarras").HasMaxLength(48);
        });

        builder.HasOne(p => p.CategoriaPeca)
            .WithMany(c => c.Pecas)
            .HasForeignKey(p => p.CategoriaId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(p => p.Fornecedor)
            .WithMany(f => f.Pecas)
            .HasForeignKey(p => p.FornecedorId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}