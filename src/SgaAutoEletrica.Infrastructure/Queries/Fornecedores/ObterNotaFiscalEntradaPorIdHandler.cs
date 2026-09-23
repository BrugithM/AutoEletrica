using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Fornecedores.DTOs;
using SgaAutoEletrica.Application.Features.Fornecedores.Queries;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Queries.Fornecedores;

public class ObterNotaFiscalEntradaPorIdHandler : IRequestHandler<ObterNotaFiscalEntradaPorIdQuery, NotaFiscalEntradaDetalheDTO?>
{
    private readonly AppDbContext _context;

    public ObterNotaFiscalEntradaPorIdHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<NotaFiscalEntradaDetalheDTO?> Handle(ObterNotaFiscalEntradaPorIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.NotasFiscaisEntrada
            .Include(nf => nf.Fornecedor)
            .Include(nf => nf.Itens).ThenInclude(i => i.Peca)
            .AsNoTracking()
            .Where(nf => nf.Id == request.Id)
            .Select(nf => new NotaFiscalEntradaDetalheDTO
            {
                Id = nf.Id,
                Numero = nf.Numero,
                DataEntrada = nf.DataEntrada,
                NomeFornecedor = nf.Fornecedor.NomeEmpresa,
                CnpjFornecedor = nf.Fornecedor.Cnpj.Formatado(),
                ValorTotal = nf.ValorTotal,
                Observacao = nf.Observacao,
                Itens = nf.Itens.Select(i => new ItemNotaEntradaDetalheDTO
                {
                    NomePeca = i.Peca.Nome,
                    Quantidade = i.Quantidade,
                    ValorUnitario = i.ValorUnitario,
                    ValorTotal = i.ValorTotal
                }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}