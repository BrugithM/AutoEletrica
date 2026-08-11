using MediatR;
namespace SgaAutoEletrica.Application.Features.Pecas.Commands;

public class ExcluirPecaCommand : IRequest
{
    public Guid Id {get;set;}
}