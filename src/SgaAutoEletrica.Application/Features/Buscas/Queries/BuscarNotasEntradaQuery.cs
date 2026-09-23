using MediatR;
using SgaAutoEletrica.Application.Common.DTOs;
using SgaAutoEletrica.Application.Features.Buscas.DTOs;

namespace SgaAutoEletrica.Application.Features.Buscas.Queries;

public class BuscarNotasEntradaQuery : IRequest<ListaPaginadaDTO<NotaFiscalEntradaResumoDTO>>
{
    public string? NomeFornecedor { get; set; }
    public string? CnpjFornecedor { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public string? CodigoProduto { get; set; }
    public string? NomeProduto { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 20;
}