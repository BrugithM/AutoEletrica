using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Domain.Entities;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Persistence.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly AppDbContext _context;

    public ClienteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Cliente?> ObterPorId(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Clientes
            .Include(c => c.Veiculos)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Cliente?> ObterPorCpf(string cpf, CancellationToken cancellationToken = default)
    {
        return await _context.Clientes
            .FirstOrDefaultAsync(c => c.Cpf.Valor == cpf, cancellationToken);
    }

    public async Task<List<Cliente>> ListarTodos(CancellationToken cancellationToken = default)
    {
        return await _context.Clientes
            .Include(c => c.Veiculos)
            .OrderBy(c => c.NomeCompleto)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Cliente>> ObterPorNome(string termo, CancellationToken cancellationToken = default)
    {
        return await _context.Clientes
            .Include(c => c.Veiculos)
            .Where(c => c.NomeCompleto.Contains(termo))
            .OrderBy(c => c.NomeCompleto)
            .ToListAsync(cancellationToken);
    }

    public async Task Adicionar(Cliente cliente, CancellationToken cancellationToken = default)
    {
        await _context.Clientes.AddAsync(cliente, cancellationToken);
    }

    public void Atualizar(Cliente cliente)
    {
        _context.Clientes.Update(cliente);
    }

    public void Remover(Cliente cliente)
    {
        _context.Clientes.Remove(cliente);
    }
}