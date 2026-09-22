using MediatR;

namespace SgaAutoEletrica.Application.Features.Clientes.Commands;

public class ReativarClienteCommand : IRequest
{
    public Guid Id { get; set; }
}