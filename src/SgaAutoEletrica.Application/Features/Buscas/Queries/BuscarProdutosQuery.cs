using MediatR;
using SgaAutoEletrica.Application.Features.Buscas.DTOs;

namespace SgaAutoEletrica.Application.Features.Buscas.Queries;

public class BuscarProdutosQuery : IRequest<List<PecaBuscaDTO>>
{
    public string? CodigoBarras { get; set; }
    public Guid? Id { get; set; }
    public string? CodigoPeca { get; set; }
    public string? Marca { get; set; }
    public Guid? CategoriaId { get; set; }
}