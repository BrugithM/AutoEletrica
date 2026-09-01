using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Domain.Entities;
using SgaAutoEletrica.Domain.Enums;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Persistence.Repositories;

public class ConfiguracaoImpressoraRepository : IConfiguracaoImpressoraRepository
{
    private readonly AppDbContext _context;

    public ConfiguracaoImpressoraRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ConfiguracaoImpressora?> ObterPorTipo(TipoImpressao tipo, CancellationToken cancellationToken = default)
    {
        return await _context.ConfiguracoesImpressora
            .FirstOrDefaultAsync(c => c.Tipo == tipo && c.Ativo, cancellationToken);
    }

    public async Task<List<ConfiguracaoImpressora>> ListarTodas(CancellationToken cancellationToken = default)
    {
        return await _context.ConfiguracoesImpressora
            .OrderBy(c => c.Tipo)
            .ToListAsync(cancellationToken);
    }

    public async Task Adicionar(ConfiguracaoImpressora config, CancellationToken cancellationToken = default)
    {
        await _context.ConfiguracoesImpressora.AddAsync(config, cancellationToken);
    }

    public void Atualizar(ConfiguracaoImpressora config)
    {
        _context.ConfiguracoesImpressora.Update(config);
    }

    public void Remover(ConfiguracaoImpressora config)
    {
        _context.ConfiguracoesImpressora.Remove(config);
    }
}