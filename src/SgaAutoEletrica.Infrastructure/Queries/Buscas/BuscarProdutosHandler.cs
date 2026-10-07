using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Buscas.DTOs;
using SgaAutoEletrica.Application.Features.Buscas.Queries;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Queries.Buscas;

public class BuscarProdutosHandler : IRequestHandler<BuscarProdutosQuery, List<PecaBuscaDTO>>
{
    private readonly AppDbContext _context;

    public BuscarProdutosHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<PecaBuscaDTO>> Handle(BuscarProdutosQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Pecas
            .Include(p => p.CategoriaPeca)
            .Include(p => p.Marca)
            .AsNoTracking()
            .AsQueryable();

        if (request.Id.HasValue)
            query = query.Where(p => p.Id == request.Id.Value);

        if (!string.IsNullOrWhiteSpace(request.CodigoBarras))
        {
            var termo = request.CodigoBarras.Trim();
            query = query.Where(p => p.CodigoBarras != null && p.CodigoBarras.Valor.Contains(termo));
        }

        if (!string.IsNullOrWhiteSpace(request.CodigoPeca))
        {
            var termo = request.CodigoPeca.Trim().ToLower();
            query = query.Where(p => p.CodigoPeca != null && p.CodigoPeca.ToLower().Contains(termo));
        }

        if (request.MarcaId.HasValue)
            query = query.Where(p => p.MarcaId == request.MarcaId.Value);

        if (request.CategoriaId.HasValue && request.CategoriaId.Value != Guid.Empty)
            query = query.Where(p => p.CategoriaId == request.CategoriaId.Value);

        return await query
            .OrderBy(p => p.Nome)
            .Select(p => new PecaBuscaDTO
            {
                Id = p.Id,
                CodigoPeca = p.CodigoPeca,
                CodigoBarras = p.CodigoBarras != null ? p.CodigoBarras.Valor : null,
                Nome = p.Nome,
                MarcaNome = p.Marca != null ? p.Marca.Nome : null,
                CategoriaNome = p.CategoriaPeca != null ? p.CategoriaPeca.Nome : null,
                ValorVenda = p.ValorVenda,
                Estoque = p.Estoque
            })
            .ToListAsync(cancellationToken);
    }
}