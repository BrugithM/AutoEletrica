using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SgaAutoEletrica.Domain.Entities;

namespace SgaAutoEletrica.Infrastructure.Persistence.Configurations;

public class FornecedorConfiguration : IEntityTypeConfiguration<Fornecedor>
{
    public void Configure(EntityTypeBuilder<Fornecedor> builder)
    {
        builder.ToTable("Fornecedores");
        builder.HasKey(f => f.Id);

        builder.Property(f => f.NomeEmpresa).IsRequired().HasMaxLength(200);
        builder.Property(f => f.Contato).HasMaxLength(100);
        builder.Property(f => f.DataCadastro).IsRequired();

         builder.OwnsOne(f => f.Cnpj, cnpj =>
        {
            cnpj.Property(c => c.Valor).HasColumnName("Cnpj").IsRequired().HasMaxLength(14);
        });

        builder.OwnsOne(f => f.Telefone, telefone =>
        {
            telefone.Property(t => t.Valor).HasColumnName("Telefone").HasMaxLength(11);
        });

        builder.OwnsOne(f => f.Endereco, endereco =>
        {
            endereco.Property(e => e.Logradouro).HasColumnName("EnderecoLogradouro").HasMaxLength(200);
            endereco.Property(e => e.Numero).HasColumnName("EnderecoNumero").HasMaxLength(20);
            endereco.Property(e => e.Complemento).HasColumnName("EnderecoComplemento").HasMaxLength(100);
            endereco.Property(e => e.Bairro).HasColumnName("EnderecoBairro").HasMaxLength(100);
            endereco.Property(e => e.Cidade).HasColumnName("EnderecoCidade").HasMaxLength(100);
            endereco.Property(e => e.Estado).HasColumnName("EnderecoEstado").HasMaxLength(2);
            endereco.Property(e => e.Cep).HasColumnName("EnderecoCep").HasMaxLength(8);
        });
    }
}
