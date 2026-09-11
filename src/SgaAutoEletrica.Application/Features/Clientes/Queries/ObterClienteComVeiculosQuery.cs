using MediatR;
using SgaAutoEletrica.Application.Features.Clientes.DTOs;

namespace SgaAutoEletrica.Application.Features.Clientes.Queries;

public class ObterClienteComVeiculosQuery : IRequest<ClienteComVeiculosDTO?>
{
    public Guid ClienteId { get; set; }
}