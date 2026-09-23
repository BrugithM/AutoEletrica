using MediatR;
using SgaAutoEletrica.Application.Features.Veiculos.DTOs;

namespace SgaAutoEletrica.Application.Features.Veiculos.Queries;

/// <summary>
/// Busca veículos por um termo único que procura em:
/// placa, modelo, marca e nome do cliente.
/// </summary>
public class BuscarVeiculosQuery : IRequest<List<VeiculoDTO>>
{
    public string? TermoBusca { get; set; }
    public bool? Ativo { get; set; } = true;

}