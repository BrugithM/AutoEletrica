using MediatR;

namespace SgaAutoEletrica.Application.Features.OrdensServico.Commands;

public class AplicarDescontoOSCommand : IRequest
{
    public Guid OrdemServicoId { get; set; }
    public decimal DescontoPercentual { get; set; }
}