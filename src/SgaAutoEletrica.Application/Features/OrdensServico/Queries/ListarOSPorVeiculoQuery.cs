using MediatR;
using SgaAutoEletrica.Application.Features.OrdensServico.DTOs;

namespace SgaAutoEletrica.Application.Features.OrdensServico.Queries;

public class ListarOSPorVeiculoQuery : IRequest<List<OrdemServicoResumoDTO>>
{
    public Guid VeiculoId { get; set; }
}