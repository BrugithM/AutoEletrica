using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Domain.Entities;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Persistence.Repositories;

public class PecaRepository : IPecaRepository
{
    private readonly AppDbContext _context;

    public PecaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Peca?> ObterPorId(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Pecas
            .Include(p => p.CategoriaPeca)
            .Include(p => p.Fornecedor)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Peca?> ObterPorCodigo(string codigo, CancellationToken cancellationToken = default)
    {
        return await _context.Pecas
            .FirstOrDefaultAsync(p => p.CodigoPeca == codigo, cancellationToken);
    }

    public async Task<Peca?> ObterPorIdPeca(string idPeca, CancellationToken cancellationToken = default)
    {
        return await _context.Pecas
            .FirstOrDefaultAsync(p => p.IdPeca == idPeca, cancellationToken);
    }

    public async Task<Peca?> ObterPorCodigoBarras(string codigoBarras, CancellationToken cancellationToken = default)
    {
        return await _context.Pecas
            .FirstOrDefaultAsync(p => p.CodigoBarras != null && p.CodigoBarras.Valor == codigoBarras, cancellationToken);
    }

    public async Task<List<Peca>> ListarTodas(CancellationToken cancellationToken = default)
    {
        return await _context.Pecas
            .Include(p => p.CategoriaPeca)
            .Include(p => p.Fornecedor)
            .OrderBy(p => p.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Peca>> ListarEstoqueBaixo(CancellationToken cancellationToken = default)
    {
        return await _context.Pecas
            .Where(p => p.Estoque <= p.EstoqueMinimo && p.Ativo)
            .OrderBy(p => p.Estoque)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Peca>> BuscarPorNome(string termo, CancellationToken cancellationToken = default)
    {
        return await _context.Pecas
            .Include(p => p.CategoriaPeca)
            .Where(p => p.Nome.Contains(termo) || p.IdPeca.Contains(termo))
            .OrderBy(p => p.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task Adicionar(Peca peca, CancellationToken cancellationToken = default)
    {
        await _context.Pecas.AddAsync(peca, cancellationToken);
    }

    public void Atualizar(Peca peca)
    {
        _context.Pecas.Update(peca);
    }

    public void Remover(Peca peca)
    {
        _context.Pecas.Remove(peca);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}