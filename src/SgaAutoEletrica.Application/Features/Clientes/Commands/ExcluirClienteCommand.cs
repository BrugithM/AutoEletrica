using MediatR;

namespace SgaAutoEletrica.Application.Features.Clientes.Commands;

public class ExcluirClienteCommand : IRequest
{
    public Guid Id { get; set; }
}