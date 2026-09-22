using MediatR;

namespace SgaAutoEletrica.Application.Features.Clientes.Commands;

public class DesativarClienteCommand : IRequest
{
    public Guid Id { get; set; }
}