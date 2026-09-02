using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SgaAutoEletrica.Domain.Entities;

namespace SgaAutoEletrica.Infrastructure.Persistence.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Nome)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.SenhaHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.Nivel)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(u => u.Ativo)
            .IsRequired();
            
        builder.Property(u => u.DataCriacao)
            .IsRequired();

        builder.HasIndex(u => u.Nome).IsUnique();
    }
}