using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Pecas.DTOs;
using SgaAutoEletrica.Application.Features.Pecas.Queries;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Queries.Pecas;

public class ListarEstoqueBaixoHandler : IRequestHandler<ListarEstoqueBaixoQuery, List<EstoqueBaixoDTO>>
{
    private readonly AppDbContext _context;

    public ListarEstoqueBaixoHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<EstoqueBaixoDTO>> Handle(ListarEstoqueBaixoQuery request, CancellationToken cancellationToken)
    {
        return await _context.Pecas
            .Include(p => p.Marca)
            .Where(p => p.Estoque <= p.EstoqueMinimo && p.Ativo)
            .OrderBy(p => p.Estoque)
            .Select(p => new EstoqueBaixoDTO
            {
                Id = p.Id,
                Nome = p.Nome,
                MarcaNome = p.Marca != null ? p.Marca.Nome : null,
                Estoque = p.Estoque,
                EstoqueMinimo = p.EstoqueMinimo
            })
            .ToListAsync(cancellationToken);
    }
}