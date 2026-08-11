using MediatR;
using SgaAutoEletrica.Application.Features.OrdensServico.DTOs;

namespace SgaAutoEletrica.Application.Features.OrdensServico.Queries;

public class ObterOSPorIdQuery : IRequest<OrdemServicoDetalheDTO?>
{
    public Guid Id { get; set; }
}