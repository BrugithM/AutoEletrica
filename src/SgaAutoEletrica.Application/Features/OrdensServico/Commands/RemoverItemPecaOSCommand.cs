using MediatR;

namespace SgaAutoEletrica.Application.Features.OrdensServico.Commands;

public class RemoverItemPecaOSCommand : IRequest
{
    public Guid OrdemServicoId { get; set; }
    public Guid ItemId { get; set; }
}