using MediatR;
using SgaAutoEletrica.Application.Features.Buscas.DTOs;

namespace SgaAutoEletrica.Application.Features.Buscas.Queries;

public class BuscarNotasEntradaQuery : IRequest<List<NotaFiscalEntradaResumoDTO>>
{
    public string? NomeFornecedor { get; set; }
    public string? CnpjFornecedor { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public string? CodigoProduto { get; set; }
    public string? NomeProduto { get; set; }
}