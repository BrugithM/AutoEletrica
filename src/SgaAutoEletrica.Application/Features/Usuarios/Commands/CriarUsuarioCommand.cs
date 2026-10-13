using MediatR;
using SgaAutoEletrica.Domain.Enums;

namespace SgaAutoEletrica.Application.Features.Usuarios.Commands;

public class CriarUsuarioCommand : IRequest<Guid>
{
    public string Nome { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public NivelUsuario Nivel { get; set; } = NivelUsuario.Operador;
}