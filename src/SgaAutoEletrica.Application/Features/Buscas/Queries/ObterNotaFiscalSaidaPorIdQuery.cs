using MediatR;
using SgaAutoEletrica.Application.Features.Buscas.DTOs;

namespace SgaAutoEletrica.Application.Features.Buscas.Queries;

public class ObterNotaFiscalSaidaPorIdQuery : IRequest<NotaFiscalSaidaDetalheDTO?>
{
    public Guid Id { get; set; }
}