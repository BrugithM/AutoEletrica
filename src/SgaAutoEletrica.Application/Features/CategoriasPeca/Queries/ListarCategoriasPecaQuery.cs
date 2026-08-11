using MediatR;
using SgaAutoEletrica.Application.Features.CategoriasPeca.DTOs;

namespace SgaAutoEletrica.Application.Features.CategoriasPeca.Queries;
public class ListarCategoriasPecaQuery : IRequest<List<CategoriaPecaDTO>> {}