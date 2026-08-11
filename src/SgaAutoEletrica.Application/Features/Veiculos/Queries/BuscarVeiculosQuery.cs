using MediatR;
using SgaAutoEletrica.Application.Features.Veiculos.DTOs;

namespace SgaAutoEletrica.Application.Features.Veiculos.Queries;

/// <summary>
/// Busca veículos por múltiplos critérios.
/// </summary>
public class BuscarVeiculosQuery : IRequest<List<VeiculoDTO>>
{
    public string? Placa { get; set; }
    public string? Marca { get; set; }
    public string? Modelo { get; set; }
    public int? Ano { get; set; }
    public string? NomeCliente { get; set; }
}