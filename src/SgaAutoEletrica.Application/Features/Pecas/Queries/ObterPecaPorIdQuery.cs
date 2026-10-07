using MediatR;
using SgaAutoEletrica.Application.Features.Pecas.DTOs;

namespace SgaAutoEletrica.Application.Features.Pecas.Queries;

public class ObterPecaPorIdQuery : IRequest<PecaDTO?>
{
    public int Id { get; set; }
}