namespace SgaAutoEletrica.Application.Features.CategoriasPeca.DTOs;

public class CategoriaPecaDTO
{
    public Guid Id {get;set;}
    public string Nome{get;set;} = string.Empty;
    public string? Descricao {get;set;}
}