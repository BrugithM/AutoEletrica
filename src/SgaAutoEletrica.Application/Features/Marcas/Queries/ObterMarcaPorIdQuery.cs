using MediatR;
using SgaAutoEletrica.Application.Features.Marcas.DTOs;

namespace SgaAutoEletrica.Application.Features.Marcas.Queries;

public class ObterMarcaPorIdQuery : IRequest<MarcaDTO?>
{
    public int Id { get; set; }
}