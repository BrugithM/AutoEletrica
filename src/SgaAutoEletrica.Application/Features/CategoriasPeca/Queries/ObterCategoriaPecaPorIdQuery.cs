using MediatR;
using SgaAutoEletrica.Application.Features.CategoriasPeca.DTOs;

namespace SgaAutoEletrica.Application.Features.CategoriasPeca.Queries;

public class ObterCategoriaPecaPorIdQuery : IRequest<CategoriaPecaDTO?>
{
    public Guid Id { get; set; }
}