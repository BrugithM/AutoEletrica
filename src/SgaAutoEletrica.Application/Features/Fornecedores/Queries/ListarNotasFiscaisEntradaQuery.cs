using MediatR;
using SgaAutoEletrica.Application.Common.DTOs;
using SgaAutoEletrica.Application.Features.Fornecedores.DTOs;

namespace SgaAutoEletrica.Application.Features.Fornecedores.Queries;

public class ListarNotasFiscaisEntradaQuery : IRequest<ListaPaginadaDTO<NotaFiscalEntradaDTO>>
{
    public string? TermoBusca { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 20;
}