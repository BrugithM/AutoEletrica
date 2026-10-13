using MediatR;

namespace SgaAutoEletrica.Application.Features.Usuarios.Commands;

public class AlterarMeuNomeCommand : IRequest
{
    public string NovoNome { get; set; } = string.Empty;
}