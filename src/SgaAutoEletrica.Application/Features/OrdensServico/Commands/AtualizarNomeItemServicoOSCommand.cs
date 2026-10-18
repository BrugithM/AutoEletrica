using MediatR;

namespace SgaAutoEletrica.Application.Features.OrdensServico.Commands;

public class AtualizarNomeItemServicoOSCommand : IRequest
{
    public Guid OrdemServicoId { get; set; }
    public Guid ItemId { get; set; }
    public string? NovoNome { get; set; }
}