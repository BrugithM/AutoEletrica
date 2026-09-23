using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Common.DTOs;
using SgaAutoEletrica.Application.Features.Fornecedores.DTOs;
using SgaAutoEletrica.Application.Features.Fornecedores.Queries;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Queries.Fornecedores;

public class ListarNotasFiscaisEntradaHandler : IRequestHandler<ListarNotasFiscaisEntradaQuery, ListaPaginadaDTO<NotaFiscalEntradaDTO>>
{
    private readonly AppDbContext _context;

    public ListarNotasFiscaisEntradaHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ListaPaginadaDTO<NotaFiscalEntradaDTO>> Handle(ListarNotasFiscaisEntradaQuery request, CancellationToken cancellationToken)
    {
        var query = _context.NotasFiscaisEntrada
            .Include(nf => nf.Fornecedor)
            .Include(nf => nf.Itens).ThenInclude(i => i.Peca)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.TermoBusca))
        {
            var termo = request.TermoBusca.Trim().ToLower();
            query = query.Where(nf =>
                nf.Numero.ToLower().Contains(termo) ||
                nf.Fornecedor.NomeEmpresa.ToLower().Contains(termo) ||
                nf.Fornecedor.Cnpj.Valor.Contains(termo) ||
                nf.Itens.Any(i => i.Peca.Nome.ToLower().Contains(termo)));
        }

        var totalItens = await query.CountAsync(cancellationToken);

        var itens = await query
            .OrderByDescending(nf => nf.DataEntrada)
            .Skip((request.Pagina - 1) * request.TamanhoPagina)
            .Take(request.TamanhoPagina)
            .Select(nf => new NotaFiscalEntradaDTO
            {
                Id = nf.Id,
                Numero = nf.Numero,
                DataEntrada = nf.DataEntrada,
                NomeFornecedor = nf.Fornecedor.NomeEmpresa,
                CnpjFornecedor = nf.Fornecedor.Cnpj.Formatado(),
                ValorTotal = nf.ValorTotal,
                Observacao = nf.Observacao
            })
            .ToListAsync(cancellationToken);

        return new ListaPaginadaDTO<NotaFiscalEntradaDTO>
        {
            Itens = itens,
            PaginaAtual = request.Pagina,
            TamanhoPagina = request.TamanhoPagina,
            TotalItens = totalItens
        };
    }
}