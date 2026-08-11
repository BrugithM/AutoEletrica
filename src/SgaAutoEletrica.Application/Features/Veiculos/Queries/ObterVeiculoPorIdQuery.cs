using MediatR;
using SgaAutoEletrica.Application.Features.Veiculos.DTOs;

namespace SgaAutoEletrica.Application.Features.Veiculos.Queries;

public class ObterVeiculoPorIdQuery : IRequest<VeiculoDTO?>
{
    public Guid Id { get; set; }
}