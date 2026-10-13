using SgaAutoEletrica.Domain.Enums;

namespace SgaAutoEletrica.Application.Features.Usuarios.DTOs;

public class UsuarioDTO
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public NivelUsuario Nivel { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? UltimoLogin { get; set; }
}