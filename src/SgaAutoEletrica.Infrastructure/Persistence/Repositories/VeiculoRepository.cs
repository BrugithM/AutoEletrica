using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Domain.Entities;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Persistence.Repositories;

public class VeiculoRepository : IVeiculoRepository
{
    private readonly AppDbContext _context;

    public VeiculoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Veiculo?> ObterPorId(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Veiculos
            .Include(v => v.Cliente)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<Veiculo?> ObterPorPlaca(string placa, CancellationToken cancellationToken = default)
    {
        return await _context.Veiculos
            .Include(v => v.Cliente)
            .FirstOrDefaultAsync(v => v.Placa.Valor == placa, cancellationToken);
    }

    public async Task<List<Veiculo>> ListarPorCliente(Guid clienteId, CancellationToken cancellationToken = default)
    {
        return await _context.Veiculos
            .Include(v => v.Cliente)
            .Where(v => v.ClienteId == clienteId)
            .OrderBy(v => v.Modelo)
            .ToListAsync(cancellationToken);
    }

    public async Task Adicionar(Veiculo veiculo, CancellationToken cancellationToken = default)
    {
        await _context.Veiculos.AddAsync(veiculo, cancellationToken);
    }

    public void Atualizar(Veiculo veiculo)
    {
        _context.Veiculos.Update(veiculo);
    }

    public void Remover(Veiculo veiculo)
    {
        _context.Veiculos.Remove(veiculo);
    }
}