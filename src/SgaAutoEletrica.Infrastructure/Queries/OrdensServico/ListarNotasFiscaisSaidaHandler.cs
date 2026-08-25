using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.OrdensServico.DTOs;
using SgaAutoEletrica.Application.Features.OrdensServico.Queries;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Queries.OrdensServico;

public class ListarNotasFiscaisSaidaHandler : IRequestHandler<ListarNotasFiscaisSaidaQuery, List<NotaFiscalSaidaDTO>>
{
    private readonly AppDbContext _context;

    public ListarNotasFiscaisSaidaHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<NotaFiscalSaidaDTO>> Handle(ListarNotasFiscaisSaidaQuery request, CancellationToken cancellationToken)
    {
        return await _context.NotasFiscaisSaida
            .Include(nf => nf.Cliente)
            .Include(nf => nf.Veiculo)
            .AsNoTracking()
            .OrderByDescending(nf => nf.DataEmissao)
            .Select(nf => new NotaFiscalSaidaDTO
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