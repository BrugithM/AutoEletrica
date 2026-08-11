using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SgaAutoEletrica.Domain.Entities;

namespace SgaAutoEletrica.Infrastructure.Persistence.Configurations;

public class ClienteConfigueation : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Clientes");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.NomeCompleto)
        .IsRequired()
        .HasMaxLength(300);

        builder.Property(c => c.DataCadastro)
        .IsRequired();

        builder.OwnsOne(c => c.Cpf, cpf =>
        {
            cpf.Property(p => p.Valor)
            .HasColumnName("Cpf")
            .IsRequired()
            .HasMaxLength(11);
        });

        builder.OwnsOne(c => c.Telefone, telefone =>
        {
            telefone.Property(t => t.Valor)
            .HasColumnName("Telefone")
            .IsRequired()
            .HasMaxLength(11);
        });

        builder.OwnsOne(c => c.Endereco, endereco =>
        {
            endereco.Property(e => e.Logradouro).HasColumnName("EnderecoLogradouro").HasMaxLength(200);
            endereco.Property(e => e.Numero).HasColumnName("EnderecoNumero").HasMaxLength(20);
            endereco.Property(e => e.Complemento).HasColumnName("EnderecoComplemento").HasMaxLength(100);
            endereco.Property(e => e.Bairro).HasColumnName("EnderecoBairro").HasMaxLength(100);
            endereco.Property(e => e.Cidade).HasColumnName("EnderecoCidade").HasMaxLength(100);
            endereco.Property(e => e.Estado).HasColumnName("EnderecoEstado").HasMaxLength(2);
            endereco.Property(e => e.Cep).HasColumnName("EnderecoCep").HasMaxLength(8);
        });

        builder.HasMany(c => c.Veiculos)
            .WithOne(v => v.Cliente)
            .HasForeignKey(v => v.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}