using MediatR;

namespace SgaAutoEletrica.Application.Features.OrdensServico.Commands;

public class AdicionarServicoOSCommand : IRequest
{
    public Guid OrdemServicoId { get; set; }
    public Guid ServicoId { get; set; }
    public decimal? PrecoManual { get; set; }
}