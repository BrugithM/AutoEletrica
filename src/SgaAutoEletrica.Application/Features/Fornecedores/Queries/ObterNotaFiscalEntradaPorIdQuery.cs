using MediatR;
using SgaAutoEletrica.Application.Features.Fornecedores.DTOs;

namespace SgaAutoEletrica.Application.Features.Fornecedores.Queries;

public class ObterNotaFiscalEntradaPorIdQuery : IRequest<NotaFiscalEntradaDetalheDTO?>
{
    public Guid Id { get; set; }
}