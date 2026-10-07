using MediatR;
using SgaAutoEletrica.Application.Features.Marcas.DTOs;

namespace SgaAutoEletrica.Application.Features.Marcas.Queries;

public class ListarMarcasQuery : IRequest<List<MarcaDTO>>
{
    public string? TermoBusca { get; set; }
    public bool? Ativo { get; set; } = true;
}