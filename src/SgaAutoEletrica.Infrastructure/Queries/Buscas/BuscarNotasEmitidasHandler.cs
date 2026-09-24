using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Common.DTOs;
using SgaAutoEletrica.Application.Features.Buscas.DTOs;
using SgaAutoEletrica.Application.Features.Buscas.Queries;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Queries.Buscas;

public class BuscarNotasEmitidasHandler : IRequestHandler<BuscarNotasEmitidasQuery, ListaPaginadaDTO<NotaFiscalSaidaResumoDTO>>
{
    private readonly AppDbContext _context;

    public BuscarNotasEmitidasHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ListaPaginadaDTO<NotaFiscalSaidaResumoDTO>> Handle(BuscarNotasEmitidasQuery request, CancellationToken cancellationToken)
    {
        var query = _context.NotasFiscaisSaida
            .Include(nf => nf.Cliente)
            .Include(nf => nf.Veiculo)
            .Include(nf => nf.Itens)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Placa))
        {
            var termo = request.Placa.Trim().ToLower();
            query = query.Where(nf => nf.Veiculo.Placa.Valor.ToLower().Contains(termo));
        }

        if (!string.IsNullOrWhiteSpace(request.NomeCliente))
        {
            var termo = request.NomeCliente.Trim().ToLower();
            query = query.Where(nf => nf.Cliente.NomeCompleto.ToLower().Contains(termo));
        }

        if (!string.IsNullOrWhiteSpace(request.NomePeca))
        {
            var termo = request.NomePeca.Trim().ToLower();
            query = query.Where(nf => nf.Itens.Any(i => i.Descricao.ToLower().Contains(termo)));
        }

        if (!string.IsNullOrWhiteSpace(request.Observacao))
        {
            var termo = request.Observacao.Trim().ToLower();
            query = query.Where(nf => nf.Observacao != null && nf.Observacao.ToLower().Contains(termo));
        }

        if (request.DataInicio.HasValue)
        {
            var data = request.DataInicio.Value.Date;
            query = query.Where(nf => nf.DataEmissao >= data);
        }

        if (request.DataFim.HasValue)
        {
            var data = request.DataFim.Value.Date.AddDays(1);
            query = query.Where(nf => nf.DataEmissao < data);
        }

        var totalItens = await query.CountAsync(cancellationToken);

        var itens = await query
            .OrderByDescending(nf => nf.DataEmissao)
            .Skip((request.Pagina - 1) * request.TamanhoPagina)
            .Take(request.TamanhoPagina)
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

        return new ListaPaginadaDTO<NotaFiscalSaidaResumoDTO>
        {
            Itens = itens,
            PaginaAtual = request.Pagina,
            TamanhoPagina = request.TamanhoPagina,
            TotalItens = totalItens
        };
    }
}