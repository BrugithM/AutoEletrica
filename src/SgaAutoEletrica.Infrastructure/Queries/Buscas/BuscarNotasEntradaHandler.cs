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
            .AsNoTracking()
            .AsQueryable();

        // Nome do Fornecedor — case-insensitive e busca parcial
        if (!string.IsNullOrWhiteSpace(request.NomeFornecedor))
        {
            var termo = request.NomeFornecedor.Trim().ToLower();
            query = query.Where(nf => nf.Fornecedor.NomeEmpresa.ToLower().Contains(termo));
        }

        // CNPJ do Fornecedor — busca parcial (removendo pontuação)
        if (!string.IsNullOrWhiteSpace(request.CnpjFornecedor))
        {
            var termo = new string(request.CnpjFornecedor.Where(char.IsDigit).ToArray());
            query = query.Where(nf => nf.Fornecedor.Cnpj.Valor.Contains(termo));
        }

        // Produto (código ou nome) — case-insensitive e busca parcial nos itens
        if (!string.IsNullOrWhiteSpace(request.CodigoProduto))
        {
            var termo = request.CodigoProduto.Trim().ToLower();
            query = query.Where(nf => nf.Itens.Any(i =>
                (i.Peca.CodigoPeca != null && i.Peca.CodigoPeca.ToLower().Contains(termo)) ||
                (i.Peca.IdPeca != null && i.Peca.IdPeca.ToLower().Contains(termo))));
        }

        if (!string.IsNullOrWhiteSpace(request.NomeProduto))
        {
            var termo = request.NomeProduto.Trim().ToLower();
            query = query.Where(nf => nf.Itens.Any(i => i.Peca.Nome.ToLower().Contains(termo)));
        }

        // Data Início
        if (request.DataInicio.HasValue)
        {
            var data = request.DataInicio.Value.Date;
            query = query.Where(nf => nf.DataEntrada >= data);
        }

        // Data Fim
        if (request.DataFim.HasValue)
        {
            var data = request.DataFim.Value.Date.AddDays(1);
            query = query.Where(nf => nf.DataEntrada < data);
        }

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