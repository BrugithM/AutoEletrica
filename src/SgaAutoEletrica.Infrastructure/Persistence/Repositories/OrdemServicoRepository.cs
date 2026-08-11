using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Domain.Entities;
using SgaAutoEletrica.Domain.Enums;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Persistence.Repositories;

public class OrdemServicoRepository : IOrdemServicoRepository
{
    private readonly AppDbContext _context;

    public OrdemServicoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<OrdemServico?> ObterPorId(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.OrdensServico
            .Include(os => os.Cliente)
            .Include(os => os.Veiculo)
            .Include(os => os.ItensPeca).ThenInclude(i => i.Peca)
            .Include(os => os.ItensServico).ThenInclude(i => i.Servico)
            .FirstOrDefaultAsync(os => os.Id == id, cancellationToken);
    }

    public async Task<OrdemServico?> ObterPorNumero(int numero, CancellationToken cancellationToken = default)
    {
        return await _context.OrdensServico
            .Include(os => os.Cliente)
            .Include(os => os.Veiculo)
            .FirstOrDefaultAsync(os => os.Numero == numero, cancellationToken);
    }

    public async Task<List<OrdemServico>> ListarTodas(CancellationToken cancellationToken = default)
    {
        return await _context.OrdensServico
            .Include(os => os.Cliente)
            .Include(os => os.Veiculo)
            .OrderByDescending(os => os.DataAbertura)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<OrdemServico>> ListarPorCliente(Guid clienteId, CancellationToken cancellationToken = default)
    {
        return await _context.OrdensServico
            .Include(os => os.Veiculo)
            .Where(os => os.ClienteId == clienteId)
            .OrderByDescending(os => os.DataAbertura)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<OrdemServico>> ListarPorVeiculo(Guid veiculoId, CancellationToken cancellationToken = default)
    {
        return await _context.OrdensServico
            .Include(os => os.Cliente)
            .Where(os => os.VeiculoId == veiculoId)
            .OrderByDescending(os => os.DataAbertura)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<OrdemServico>> ListarPorStatus(StatusOS status, CancellationToken cancellationToken = default)
    {
        return await _context.OrdensServico
            .Include(os => os.Cliente)
            .Include(os => os.Veiculo)
            .Where(os => os.Status == status)
            .OrderByDescending(os => os.DataAbertura)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> ObterProximoNumero(CancellationToken cancellationToken = default)
    {
        var ultimoNumero = await _context.OrdensServico
            .MaxAsync(os => (int?)os.Numero, cancellationToken) ?? 0;
        return ultimoNumero + 1;
    }

    public async Task Adicionar(OrdemServico os, CancellationToken cancellationToken = default)
    {
        await _context.OrdensServico.AddAsync(os, cancellationToken);
    }

    public void Atualizar(OrdemServico os)
    {
        _context.OrdensServico.Update(os);
    }
}