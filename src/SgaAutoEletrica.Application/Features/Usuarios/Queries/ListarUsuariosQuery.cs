using MediatR;
using SgaAutoEletrica.Application.Features.Usuarios.DTOs;

namespace SgaAutoEletrica.Application.Features.Usuarios.Queries;

public class ListarUsuariosQuery : IRequest<List<UsuarioDTO>> { }