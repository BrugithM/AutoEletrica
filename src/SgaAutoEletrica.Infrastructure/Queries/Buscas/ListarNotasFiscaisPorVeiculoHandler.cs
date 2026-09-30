using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Buscas.DTOs;
using SgaAutoEletrica.Application.Features.Buscas.Queries;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Queries.Buscas;

public class ListarNotasFiscaisPorVeiculoHandler : IRequestHandler<ListarNotasFiscaisPorVeiculoQuery, List<NotaFiscalSaidaResumoDTO>>
{
    private readonly AppDbContext _context;

    public ListarNotasFiscaisPorVeiculoHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<NotaFiscalSaidaResumoDTO>> Handle(ListarNotasFiscaisPorVeiculoQuery request, CancellationToken cancellationToken)
    {
        return await _context.NotasFiscaisSaida
            .Include(nf => nf.Cliente)
            .Include(nf => nf.Veiculo)
            .AsNoTracking()
            .Where(nf => nf.VeiculoId == request.VeiculoId)
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