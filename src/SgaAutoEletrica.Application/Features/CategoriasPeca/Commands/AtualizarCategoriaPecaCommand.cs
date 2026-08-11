using MediatR;

namespace SgaAutoEletrica.Application.Features.CategoriasPeca.Commands;

public class AtualizarCategoriaPecaCommand : IRequest
{
    public Guid Id {get; set;}
    public string Nome{get;set;}=string.Empty;
    public string? Descricao {get;set;}
}