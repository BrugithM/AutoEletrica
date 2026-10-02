using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Dashboard.DTOs;
using SgaAutoEletrica.Application.Features.Dashboard.Queries;
using SgaAutoEletrica.Domain.Enums;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Queries.Dashboard;

public class ObterResumoMensalHandler : IRequestHandler<ObterResumoMensalQuery, ResumoMensalDTO>
{
    private readonly AppDbContext _context;

    public ObterResumoMensalHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ResumoMensalDTO> Handle(ObterResumoMensalQuery request, CancellationToken cancellationToken)
    {
        var inicio = new DateTime(request.Ano, request.Mes, 1);
        var fim = inicio.AddMonths(1);

        var osFinalizadas = await _context.OrdensServico
            .Where(os => os.Status == StatusOS.Finalizada
                && os.DataFinalizacao >= inicio
                && os.DataFinalizacao < fim)
            .ToListAsync(cancellationToken);

        var faturamento = osFinalizadas.Sum(os => os.ValorTotal);
        var totalFinalizadas = osFinalizadas.Count;
        var ticket = totalFinalizadas > 0 ? faturamento / totalFinalizadas : 0;

        var osAbertas = await _context.OrdensServico
            .CountAsync(os => os.Status == StatusOS.Aberta, cancellationToken);

        var aguardandoPecas = await _context.OrdensServico
            .CountAsync(os => os.Status == StatusOS.AguardandoPecas, cancellationToken);

        var estoqueBaixo = await _context.Pecas
            .CountAsync(p => p.Estoque <= p.EstoqueMinimo && p.Ativo, cancellationToken);

        return new ResumoMensalDTO
        {
            Mes = request.Mes,
            Ano = request.Ano,
            FaturamentoTotal = faturamento,
            OSAbertas = osAbertas,
            OSFinalizadas = totalFinalizadas,
            TicketMedio = ticket,
            ProdutosEstoqueBaixo = estoqueBaixo,
            OSAguardandoPecas = aguardandoPecas
        };
    }
}