using MediatR;
using SgaAutoEletrica.Application.Features.Clientes.DTOs;

namespace SgaAutoEletrica.Application.Features.Clientes.Queries;

public class ListarTodosClientesQuery : IRequest<List<ClienteResumoDTO>> { }