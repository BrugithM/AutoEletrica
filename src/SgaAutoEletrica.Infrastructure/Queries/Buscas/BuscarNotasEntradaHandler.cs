using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Buscas.DTOs;
using SgaAutoEletrica.Application.Features.Buscas.Queries;

using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Queries.Buscas;

public class BuscarNotasEntradaHandler : IRequestHandler<BuscarNotasEntradaQuery, List<NotaFiscalEntradaResumoDTO>>
{
    private readonly AppDbContext _context;

    public BuscarNotasEntradaHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<NotaFiscalEntradaResumoDTO>> Handle(BuscarNotasEntradaQuery request, CancellationToken cancellationToken)
    {
        var query = _context.NotasFiscaisEntrada
            .Include(nf => nf.Fornecedor)
            .Include(nf => nf.Itens).ThenInclude(i => i.Peca)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.NomeFornecedor))
            query = query.Where(nf => nf.Fornecedor.NomeEmpresa.Contains(request.NomeFornecedor));

        if (!string.IsNullOrWhiteSpace(request.CnpjFornecedor))
            query = query.Where(nf => nf.Fornecedor.Cnpj.Valor == request.CnpjFornecedor);

        if (request.DataInicio.HasValue)
            query = query.Where(nf => nf.DataEntrada >= request.DataInicio.Value);

        if (request.DataFim.HasValue)
            query = query.Where(nf => nf.DataEntrada <= request.DataFim.Value);

        if (!string.IsNullOrWhiteSpace(request.CodigoProduto))
            query = query.Where(nf => nf.Itens.Any(i => i.Peca.CodigoPeca != null && i.Peca.CodigoPeca.Contains(request.CodigoProduto)));

        if (!string.IsNullOrWhiteSpace(request.NomeProduto))
            query = query.Where(nf => nf.Itens.Any(i => i.Peca.Nome.Contains(request.NomeProduto)));

        return await query
            .OrderByDescending(nf => nf.DataEntrada)
            .Select(nf => new NotaFiscalEntradaResumoDTO
            {
                Id = nf.Id,
                Numero = nf.Numero,
                DataEntrada = nf.DataEntrada,
                NomeFornecedor = nf.Fornecedor.NomeEmpresa,
                CnpjFornecedor = nf.Fornecedor.Cnpj.Valor,
                ValorTotal = nf.ValorTotal,
                Observacao = nf.Observacao
            })
            .ToListAsync(cancellationToken);
    }
}