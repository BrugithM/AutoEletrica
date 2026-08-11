using MediatR;
using SgaAutoEletrica.Application.Features.Pecas.DTOs;

namespace SgaAutoEletrica.Application.Features.Pecas.Queries;

public class ListarPecasQuery : IRequest<List<PecaDTO>>
{
    public string? TermoBusca { get; set; }
    public Guid? CategoriaId { get; set; }
    public bool? Ativo { get; set; }
}