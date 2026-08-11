using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Domain.Entities;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Persistence.Repositories;

public class ServicoRepository : IServicoRepository
{
    private readonly AppDbContext _context;

    public ServicoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Servico?> ObterPorId(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Servicos.FindAsync([id], cancellationToken);
    }

    public async Task<List<Servico>> ListarTodos(CancellationToken cancellationToken = default)
    {
        return await _context.Servicos
            .OrderBy(s => s.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Servico>> BuscarPorNome(string termo, CancellationToken cancellationToken = default)
    {
        return await _context.Servicos
            .Where(s => s.Nome.Contains(termo))
            .OrderBy(s => s.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task Adicionar(Servico servico, CancellationToken cancellationToken = default)
    {
        await _context.Servicos.AddAsync(servico, cancellationToken);
    }

    public void Atualizar(Servico servico)
    {
        _context.Servicos.Update(servico);
    }

    public void Remover(Servico servico)
    {
        _context.Servicos.Remove(servico);
    }
}