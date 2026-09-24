using MediatR;
using SgaAutoEletrica.Application.Common.DTOs;
using SgaAutoEletrica.Application.Features.Buscas.DTOs;

namespace SgaAutoEletrica.Application.Features.Buscas.Queries;

public class BuscarNotasEmitidasQuery : IRequest<ListaPaginadaDTO<NotaFiscalSaidaResumoDTO>>
{
    public string? Placa { get; set; }
    public string? NomeCliente { get; set; }
    public string? NomePeca { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public string? Observacao { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 20;
}