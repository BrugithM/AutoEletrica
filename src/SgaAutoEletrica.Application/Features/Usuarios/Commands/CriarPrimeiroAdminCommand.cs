using MediatR;
using SgaAutoEletrica.Domain.Entities;

namespace SgaAutoEletrica.Application.Features.Usuarios.Commands;

public class CriarPrimeiroAdminCommand : IRequest<Usuario>
{
    public string Nome { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}