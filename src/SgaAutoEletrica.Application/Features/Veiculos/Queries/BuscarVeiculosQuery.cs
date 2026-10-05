using MediatR;
using SgaAutoEletrica.Application.Common.DTOs;
using SgaAutoEletrica.Application.Features.Veiculos.DTOs;

namespace SgaAutoEletrica.Application.Features.Veiculos.Queries;

public class BuscarVeiculosQuery : IRequest<ListaPaginadaDTO<VeiculoDTO>>
{
    public string? TermoBusca { get; set; }

    public string? Placa { get; set; }
    public string? Modelo { get; set; }
    public string? Marca { get; set; }
    public string? NomeCliente { get; set; }
    public int? Ano { get; set; }

    public bool? Ativo { get; set; } = true;
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 20;
}