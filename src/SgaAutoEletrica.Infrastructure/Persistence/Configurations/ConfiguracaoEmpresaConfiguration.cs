using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SgaAutoEletrica.Domain.Entities;

namespace SgaAutoEletrica.Infrastructure.Persistence.Configurations;

public class ConfiguracaoEmpresaConfiguration : IEntityTypeConfiguration<ConfiguracaoEmpresa>
{
    public void Configure(EntityTypeBuilder<ConfiguracaoEmpresa> builder)
    {
        builder.ToTable("ConfiguracoesEmpresa");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.NomeEmpresa).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Cnpj).IsRequired().HasMaxLength(18);
        builder.Property(c => c.Telefone).IsRequired().HasMaxLength(15);
        builder.Property(c => c.Endereco).HasMaxLength(300);
        builder.Property(c => c.Email).HasMaxLength(100);
    }
}