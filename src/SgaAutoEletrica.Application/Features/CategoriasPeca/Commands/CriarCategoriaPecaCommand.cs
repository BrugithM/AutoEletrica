using MediatR;

namespace SgaAutoEletrica.Application.Features.CategoriasPeca.Commands;

public class CriarCategoriaPecaCommand : IRequest<Guid>
{
 public string Nome {get;set;} = string.Empty;
 public string? Descricao{get; set;}   
}