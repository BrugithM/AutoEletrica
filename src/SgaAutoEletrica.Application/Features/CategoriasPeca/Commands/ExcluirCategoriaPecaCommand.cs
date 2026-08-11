using MediatR;

namespace SgaAutoEletrica.Application.Features.CategoriasPeca.Commands;

public class ExcluirCategoriaPecaCommand : IRequest
{
    public Guid Id{get;set;}
}