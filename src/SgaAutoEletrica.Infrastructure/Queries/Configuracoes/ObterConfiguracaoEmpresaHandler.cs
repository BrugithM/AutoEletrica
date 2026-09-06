using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Configuracoes.DTOs;
using SgaAutoEletrica.Application.Features.Configuracoes.Queries;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Queries.Configuracoes;

public class ObterConfiguracaoEmpresaHandler : IRequestHandler<ObterConfiguracaoEmpresaQuery, ConfiguracaoEmpresaDTO?>
{
    private readonly AppDbContext _context;

    public ObterConfiguracaoEmpresaHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ConfiguracaoEmpresaDTO?> Handle(ObterConfiguracaoEmpresaQuery request, CancellationToken cancellationToken)
    {
        return await _context.ConfiguracoesEmpresa
            .AsNoTracking()
            .Select(c => new ConfiguracaoEmpresaDTO
            {
                Id = c.Id,
                NomeEmpresa = c.NomeEmpresa,
                Cnpj = c.Cnpj,
                Telefone = c.Telefone,
                Endereco = c.Endereco,
                Email = c.Email
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}