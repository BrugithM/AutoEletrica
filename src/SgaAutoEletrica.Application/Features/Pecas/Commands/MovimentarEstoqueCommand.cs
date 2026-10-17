using MediatR;
namespace SgaAutoEletrica.Application.Features.Pecas.Commands;

public class MovimentarEstoqueCommand : IRequest
{
    public int PecaId{get;set;}
    public decimal Quantidade {get; set;}
    public bool Entrada {get; set;}
}