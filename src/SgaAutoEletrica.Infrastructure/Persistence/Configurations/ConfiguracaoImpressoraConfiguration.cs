using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SgaAutoEletrica.Domain.Entities;

namespace SgaAutoEletrica.Infrastructure.Persistence.Configurations;

public class ConfiguracaoImpressoraConfiguration : IEntityTypeConfiguration<ConfiguracaoImpressora>
{
    public void Configure(EntityTypeBuilder<ConfiguracaoImpressora> builder)
    {
        builder.ToTable("ConficuracoesImpressora");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Tipo)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(c => c.NomeImpressora)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(c => c.TamanhoPapel)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(c => c.Copias)
            .HasDefaultValue(1);

        builder.Property(c => c.MargemSuperior).HasMaxLength(20);
        builder.Property(c => c.MargemInferior).HasMaxLength(20);
        builder.Property(c => c.MargemEsquerda).HasMaxLength(20);
        builder.Property(c => c.MargemDireita).HasMaxLength(20);

        builder.Property(c=> c.Ativo).IsRequired();

        builder.HasIndex(c => c.Tipo).IsUnique();
    }
}