using MediatR;
using SgaAutoEletrica.Application.Features.Clientes.DTOs;

namespace SgaAutoEletrica.Application.Features.Clientes.Queries;

public class ListarClientesQuery : IRequest<List<ClienteDTO>>
{
    public string? TermoBusca { get; set; }
    public bool? Ativo { get; set; } = true;
}