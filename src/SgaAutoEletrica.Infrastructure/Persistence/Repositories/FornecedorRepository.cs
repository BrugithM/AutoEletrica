using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Domain.Entities;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Persistence.Repositories;

public class FornecedorRepository : IFornecedorRepository
{
    private readonly AppDbContext _context;

    public FornecedorRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Fornecedor?> ObterPorId(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Fornecedores.FindAsync([id], cancellationToken);
    }

    public async Task<Fornecedor?> ObterPorCnpj(string cnpj, CancellationToken cancellationToken = default)
    {
        return await _context.Fornecedores
            .FirstOrDefaultAsync(f => f.Cnpj.Valor == cnpj, cancellationToken);
    }

    public async Task<List<Fornecedor>> ListarTodos(CancellationToken cancellationToken = default)
    {
        return await _context.Fornecedores
            .OrderBy(f => f.NomeEmpresa)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Fornecedor>> BuscarPorNome(string termo, CancellationToken cancellationToken = default)
    {
        return await _context.Fornecedores
            .Where(f => f.NomeEmpresa.Contains(termo))
            .OrderBy(f => f.NomeEmpresa)
            .ToListAsync(cancellationToken);
    }

    public async Task Adicionar(Fornecedor fornecedor, CancellationToken cancellationToken = default)
    {
        await _context.Fornecedores.AddAsync(fornecedor, cancellationToken);
    }

    public void Atualizar(Fornecedor fornecedor)
    {
        _context.Fornecedores.Update(fornecedor);
    }

    public void Remover(Fornecedor fornecedor)
    {
        _context.Fornecedores.Remove(fornecedor);
    }
}