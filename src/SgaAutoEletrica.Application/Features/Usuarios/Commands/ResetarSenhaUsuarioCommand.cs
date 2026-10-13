using MediatR;

namespace SgaAutoEletrica.Application.Features.Usuarios.Commands;

public class ResetarSenhaUsuarioCommand : IRequest
{
    public Guid UsuarioId { get; set; }
    public string NovaSenha { get; set; } = string.Empty;
}