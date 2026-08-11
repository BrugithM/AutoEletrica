using MediatR;
using SgaAutoEletrica.Application.Features.OrdensServico.DTOs;
using SgaAutoEletrica.Domain.Enums;

namespace SgaAutoEletrica.Application.Features.OrdensServico.Queries;

public class ListarOSQuery : IRequest<List<OrdemServicoResumoDTO>>
{
    public StatusOS? Status { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public string? TermoBusca { get; set; }
}