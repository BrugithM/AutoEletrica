using MediatR;
using SgaAutoEletrica.Application.Features.OrdensServico.DTOs;

namespace SgaAutoEletrica.Application.Features.OrdensServico.Queries;

public class ListarOSPorClienteQuery : IRequest<List<OrdemServicoResumoDTO>>
{
    public Guid ClienteId { get; set; }
}