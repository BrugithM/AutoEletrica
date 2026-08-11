using MediatR;
using SgaAutoEletrica.Application.Features.Clientes.DTOs;

namespace SgaAutoEletrica.Application.Features.Clientes.Queries;

public class ObterClientePorIdQuery : IRequest<ClienteDTO?>
{
    public Guid Id { get; set; }
}