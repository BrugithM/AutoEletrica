using MediatR;

namespace SgaAutoEletrica.Application.Features.OrdensServico.Commands;

public class RemoverItemServicoOSCommand : IRequest
{
    public Guid OrdemServicoId { get; set; }
    public Guid ItemId { get; set; }
}