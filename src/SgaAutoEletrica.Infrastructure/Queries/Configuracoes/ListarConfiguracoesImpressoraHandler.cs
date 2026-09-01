using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Configuracoes.DTOs;
using SgaAutoEletrica.Application.Features.Configuracoes.Queries;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Queries.Configuracoes;

public class ListarConfiguracoesImpressoraHandler : IRequestHandler<ListarConfiguracoesImpressoraQuery, List<ConfiguracaoImpressoraDTO>>
{
    private readonly AppDbContext _context;

    public ListarConfiguracoesImpressoraHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ConfiguracaoImpressoraDTO>> Handle(ListarConfiguracoesImpressoraQuery request, CancellationToken cancellationToken)
    {
        return await _context.ConfiguracoesImpressora
            .AsNoTracking()
            .OrderBy(c => c.Tipo)
            .Select(c => new ConfiguracaoImpressoraDTO
            {
                Id = c.Id,
                Tipo = c.Tipo,
                NomeImpressora = c.NomeImpressora,
                TamanhoPapel = c.TamanhoPapel,
                Copias = c.Copias,
                MargemSuperior = c.MargemSuperior,
                MargemInferior = c.MargemInferior,
                MargemEsquerda = c.MargemEsquerda,
                MargemDireita = c.MargemDireita
            })
            .ToListAsync(cancellationToken);
    }
}