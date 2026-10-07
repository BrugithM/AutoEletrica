using MediatR;

namespace SgaAutoEletrica.Application.Features.OrdensServico.Commands;

public class AdicionarPecaOSCommand : IRequest
{
    public Guid OrdemServicoId { get; set; }
    public int PecaId { get; set; }
    public int Quantidade { get; set; }
}