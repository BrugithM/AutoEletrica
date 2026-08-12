using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Buscas.DTOs;
using SgaAutoEletrica.Application.Features.Buscas.Queries;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Queries.Buscas;

public class BuscarNotasEmitidasHandler : IRequestHandler<BuscarNotasEmitidasQuery, List<NotaFiscalSaidaResumoDTO>>
{
    private readonly AppDbContext _context;

    public BuscarNotasEmitidasHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<NotaFiscalSaidaResumoDTO>> Handle(BuscarNotasEmitidasQuery request, CancellationToken cancellationToken)
    {
        var query = _context.NotasFiscaisSaida
            .Include(nf => nf.Cliente)
            .Include(nf => nf.Veiculo)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Placa))
            query = query.Where(nf => nf.Veiculo.Placa.Valor.Contains(request.Placa));

        if (!string.IsNullOrWhiteSpace(request.NomeCliente))
            query = query.Where(nf => nf.Cliente.NomeCompleto.Contains(request.NomeCliente));

        if (request.DataInicio.HasValue)
            query = query.Where(nf => nf.DataEmissao >= request.DataInicio.Value);

        if (request.DataFim.HasValue)
            query = query.Where(nf => nf.DataEmissao <= request.DataFim.Value);

        if (!string.IsNullOrWhiteSpace(request.Observacao))
            query = query.Where(nf => nf.Observacao != null && nf.Observacao.Contains(request.Observacao));

        if (!string.IsNullOrWhiteSpace(request.CodigoPeca) || !string.IsNullOrWhiteSpace(request.NomePeca))
        {
            query = query.Where(nf => nf.Itens.Any(i =>
                (request.CodigoPeca == null || i.Descricao.Contains(request.CodigoPeca)) &&
                (request.NomePeca == null || i.Descricao.Contains(request.NomePeca))));
        }

        return await query
            .OrderByDescending(nf => nf.DataEmissao)
            .Select(nf => new NotaFiscalSaidaResumoDTO
            {
                Id = nf.Id,
                Numero = nf.Numero,
                DataEmissao = nf.DataEmissao,
                NomeCliente = nf.Cliente.NomeCompleto,
                PlacaVeiculo = nf.Veiculo.Placa.Valor,
                ValorTotal = nf.ValorTotal,
                Observacao = nf.Observacao
            })
            .ToListAsync(cancellationToken);
    }
}