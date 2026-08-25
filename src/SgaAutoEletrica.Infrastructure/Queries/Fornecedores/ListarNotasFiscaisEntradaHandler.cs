using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Fornecedores.DTOs;
using SgaAutoEletrica.Application.Features.Fornecedores.Queries;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Queries.Fornecedores;

public class ListarNotasFiscaisEntradaHandler : IRequestHandler<ListarNotasFiscaisEntradaQuery, List<NotaFiscalEntradaDTO>>
{
    private readonly AppDbContext _context;

    public ListarNotasFiscaisEntradaHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<NotaFiscalEntradaDTO>> Handle(ListarNotasFiscaisEntradaQuery request, CancellationToken cancellationToken)
    {
        return await _context.NotasFiscaisEntrada
            .Include(nf => nf.Fornecedor)
            .AsNoTracking()
            .OrderByDescending(nf => nf.DataEntrada)
            .Select(nf => new NotaFiscalEntradaDTO
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