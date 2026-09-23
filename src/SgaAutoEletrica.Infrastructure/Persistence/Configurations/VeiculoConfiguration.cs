using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SgaAutoEletrica.Domain.Entities;

namespace SgaAutoEletrica.Infrastructure.Persistence.Configurations;

public class VeiculoConfiguration : IEntityTypeConfiguration<Veiculo>
{
    public void Configure(EntityTypeBuilder<Veiculo> builder)
    {
        builder.ToTable("Veiculos");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Modelo).IsRequired().HasMaxLength(100);
        builder.Property(v => v.Marca).IsRequired().HasMaxLength(50);
        builder.Property(v => v.Ano).IsRequired();
        builder.Property(v => v.Versao).HasMaxLength(100);
        builder.Property(v => v.Motor).HasMaxLength(100);
        builder.Property(v => v.Cor).HasMaxLength(50);
        builder.Property(v => v.Observacao).HasMaxLength(500);
        builder.Property(v => v.Ativo).IsRequired();

        builder.Property(v => v.TipoMotor)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.OwnsOne(v => v.Placa, placa =>
        {
            placa.Property(p => p.Valor)
                .HasColumnName("Placa")
                .IsRequired()
                .HasMaxLength(7);
        });

        builder.HasOne(v => v.Cliente)
            .WithMany(c => c.Veiculos)
            .HasForeignKey(v => v.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}