using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Marcas.DTOs;
using SgaAutoEletrica.Application.Features.Marcas.Queries;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Queries.Marcas;

public class ListarMarcasHandler : IRequestHandler<ListarMarcasQuery, List<MarcaDTO>>
{
    private readonly AppDbContext _context;

    public ListarMarcasHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<MarcaDTO>> Handle(ListarMarcasQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Marcas.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.TermoBusca))
        {
            var termo = request.TermoBusca.Trim().ToLower();
            query = query.Where(m => m.Nome.ToLower().Contains(termo));
        }

        if (request.Ativo.HasValue)
            query = query.Where(m => m.Ativo == request.Ativo.Value);

        return await query
            .OrderBy(m => m.Nome)
            .Select(m => new MarcaDTO
            {
                Id = m.Id,
                Nome = m.Nome,
                Ativo = m.Ativo
            })
            .ToListAsync(cancellationToken);
    }
}