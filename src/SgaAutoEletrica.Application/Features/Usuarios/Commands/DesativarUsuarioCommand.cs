using MediatR;

namespace SgaAutoEletrica.Application.Features.Usuarios.Commands;

public class DesativarUsuarioCommand : IRequest
{
    public Guid UsuarioId { get; set; }
}