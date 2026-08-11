using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Dashboard.DTOs;
using SgaAutoEletrica.Domain.Enums;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Application.Features.Dashboard.Queries;

public class ObterFaturamentoMensalHandler : IRequestHandler<ObterFaturamentoMensalQuery, List<FaturamentoMensalDTO>>
{
    private readonly AppDbContext _context;
    public ObterFaturamentoMensalHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<FaturamentoMensalDTO>> Handle(ObterFaturamentoMensalQuery request, CancellationToken cancellationToken)
    {
        var dataInicio = new DateTime(request.Ano, request.Mes, 1);
        var dataFim = dataInicio.AddMonths(1);

        return await _context.OrdensServico
            .Where(os => os.Status == StatusOS.Finalizada && os.DataFinalizacao >= dataInicio && os.DataFinalizacao < dataFim)
            .GroupBy(_ => new{request.Mes, request.Ano})
            .Select(g => new FaturamentoMensalDTO
            {
                Mes = g.Key.Mes,
                Ano = g.Key.Ano,
                TotalPecas = g.Sum(os => os.ValorTotalPecas),
                TotalServicos = g.Sum(os => os.ValorTotalServicos),
                TotalGeral = g.Sum(os => os.ValorTotal),
                QuantidadeOS = g.Count()
            })
            .ToListAsync(cancellationToken);
    }
}