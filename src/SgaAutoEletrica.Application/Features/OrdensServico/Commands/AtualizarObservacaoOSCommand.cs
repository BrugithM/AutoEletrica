using MediatR;

namespace SgaAutoEletrica.Application.Features.OrdensServico.Commands;

public class AtualizarObservacaoOSCommand : IRequest
{
    public Guid OrdemServicoId { get; set; }
    public string Observacao { get; set; } = string.Empty;
}