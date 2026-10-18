using MediatR;

namespace SgaAutoEletrica.Application.Features.OrdensServico.Commands;

public class AtualizarQuantidadeItemOSCommand : IRequest
{
    public Guid OrdemServicoId { get; set; }
    public Guid ItemId { get; set; }
    public decimal NovaQuantidade { get; set; }
}