using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Dashboard.DTOs;
using SgaAutoEletrica.Application.Features.Dashboard.Queries;
using SgaAutoEletrica.Domain.Enums;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Queries.Dashboard;

public class ObterResumoAnualHandler : IRequestHandler<ObterResumoAnualQuery, ResumoAnualDTO>
{
    private readonly AppDbContext _context;

    public ObterResumoAnualHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ResumoAnualDTO> Handle(ObterResumoAnualQuery request, CancellationToken cancellationToken)
    {
        var inicio = new DateTime(request.Ano, 1, 1);
        var fim = new DateTime(request.Ano + 1, 1, 1);

        var osFinalizadas = await _context.OrdensServico
            .Where(os => os.Status == StatusOS.Finalizada
                && os.DataFinalizacao >= inicio
                && os.DataFinalizacao < fim)
            .ToListAsync(cancellationToken);

        var faturamento = osFinalizadas.Sum(os => os.ValorTotal);
        var total = osFinalizadas.Count;
        var ticket = total > 0 ? faturamento / total : 0;

        return new ResumoAnualDTO
        {
            Ano = request.Ano,
            FaturamentoTotal = faturamento,
            OSFinalizadas = total,
            TicketMedio = ticket
        };
    }
}