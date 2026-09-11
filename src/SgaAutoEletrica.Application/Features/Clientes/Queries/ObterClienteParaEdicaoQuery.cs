using MediatR;
using SgaAutoEletrica.Application.Features.Clientes.DTOs;

namespace SgaAutoEletrica.Application.Features.Clientes.Queries;

public class ObterClienteParaEdicaoQuery : IRequest<ClienteEdicaoDTO?>
{
    public Guid Id { get; set; }
}