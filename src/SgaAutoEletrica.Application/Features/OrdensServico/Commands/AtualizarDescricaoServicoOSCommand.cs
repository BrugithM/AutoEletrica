using MediatR;

namespace SgaAutoEletrica.Application.Features.OrdensServico.Commands;

public class AtualizarDescricaoServicosOSCommand : IRequest
{
    public Guid OrdemServicoId { get; set; }
    public string? DescricaoServicos { get; set; }
}