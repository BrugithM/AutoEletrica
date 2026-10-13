using MediatR;

namespace SgaAutoEletrica.Application.Features.Usuarios.Commands;

public class AlterarMinhaSenhaCommand : IRequest
{
    public string SenhaAtual { get; set; } = string.Empty;
    public string NovaSenha { get; set; } = string.Empty;
}