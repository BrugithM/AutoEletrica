using MediatR;
using SgaAutoEletrica.Domain.Enums;

namespace SgaAutoEletrica.Application.Features.OrdensServico.Commands;

public class AlterarStatusOSCommand : IRequest
{
    public Guid OrdemServicoId { get; set; }
    public StatusOS NovoStatus { get; set; }
    public string? MotivoCancelamento { get; set; }
}