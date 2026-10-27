using MediatR;
using SgaAutoEletrica.Application.Features.Pecas.DTOs;

namespace SgaAutoEletrica.Application.Features.Pecas.Queries;

public class ListarTodasPecasQuery : IRequest<List<PecaDTO>> { }