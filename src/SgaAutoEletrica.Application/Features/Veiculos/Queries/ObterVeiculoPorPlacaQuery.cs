using MediatR;
using SgaAutoEletrica.Application.Features.Veiculos.DTOs;

namespace SgaAutoEletrica.Application.Features.Veiculos.Queries;

public class ObterVeiculoPorPlacaQuery : IRequest<VeiculoDTO?>
{
    public string Placa { get; set; } = string.Empty;
}