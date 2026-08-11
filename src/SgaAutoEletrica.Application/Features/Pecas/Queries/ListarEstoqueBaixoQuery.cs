using MediatR;
using SgaAutoEletrica.Application.Features.Pecas.DTOs;

namespace SgaAutoEletrica.Application.Features.Pecas.Queries;

public class ListarEstoqueBaixoQuery : IRequest<List<EstoqueBaixoDTO>> { }