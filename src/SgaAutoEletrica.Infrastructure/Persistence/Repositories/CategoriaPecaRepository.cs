using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Domain.Entities;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Persistence.Repositories;

public class CategoriaPecaRepository : ICategoriaPecaRepository
{
    private readonly AppDbContext _context;

    public CategoriaPecaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CategoriaPeca?> ObterPorId(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.CategoriasPecas.FindAsync([id], cancellationToken);
    }

    public async Task<List<CategoriaPeca>> ListarTodas(CancellationToken cancellationToken = default)
    {
        return await _context.CategoriasPecas
            .OrderBy(c => c.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task Adicionar(CategoriaPeca categoria, CancellationToken cancellationToken = default)
    {
        await _context.CategoriasPecas.AddAsync(categoria, cancellationToken);
    }

    public void Atualizar(CategoriaPeca categoria)
    {
        _context.CategoriasPecas.Update(categoria);
    }

    public void Remover(CategoriaPeca categoria)
    {
        _context.CategoriasPecas.Remove(categoria);
    }
}