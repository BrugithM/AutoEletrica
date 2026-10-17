using MediatR;

namespace SgaAutoEletrica.Application.Features.OrdensServico.Commands;

public class AtualizarPrecoItemOSCommand : IRequest
{
    public Guid OrdemServicoId { get; set; }
    public Guid ItemId { get; set; }
    public bool EhPeca { get; set; }  
    public decimal NovoPreco { get; set; }
}