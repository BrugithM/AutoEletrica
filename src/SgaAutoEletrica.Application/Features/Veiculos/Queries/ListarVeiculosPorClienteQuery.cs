using MediatR;
using SgaAutoEletrica.Application.Features.Veiculos.DTOs;

namespace SgaAutoEletrica.Application.Features.Veiculos.Queries;

public class ListarVeiculosPorClienteQuery : IRequest<List<VeiculoDTO>>
{
    public Guid ClienteId { get; set; }
    public bool? Ativo { get; set; } = true;

}