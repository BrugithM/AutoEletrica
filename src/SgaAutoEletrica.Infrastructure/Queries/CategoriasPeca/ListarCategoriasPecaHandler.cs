using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.CategoriasPeca.DTOs;
using SgaAutoEletrica.Application.Features.CategoriasPeca.Queries;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Queries.CategoriasPeca;

public class ListarCategoriasPecaHandler : IRequestHandler<ListarCategoriasPecaQuery, List<CategoriaPecaDTO>>
{
    private readonly AppDbContext _context;

    public ListarCategoriasPecaHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<CategoriaPecaDTO>> Handle(ListarCategoriasPecaQuery request, CancellationToken cancellationToken)
    {
        return await _context.CategoriasPecas
            .OrderBy(c => c.Nome)
            .Select(c => new CategoriaPecaDTO
            {
                Id = c.Id,
                Nome = c.Nome,
                Descricao = c.Descricao
            })
            .ToListAsync(cancellationToken);
    }
}