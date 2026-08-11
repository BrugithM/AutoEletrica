using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Dashboard.DTOs;
using SgaAutoEletrica.Domain.Enums;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Application.Features.Dashboard.Queries;

public class ObterResumoDiarioHandler : IRequestHandler<ObterResumoDiarioQuery, ResumoDiarioDTO>
{
    private readonly AppDbContext _context;
    public ObterResumoDiarioHandler(AppDbContext context)
    {
        _context = context;
    }
    public async Task<ResumoDiarioDTO> Handle(ObterResumoDiarioQuery request, CancellationToken cancellationToken)
    {
        var hoje = DateTime.Today;
        return new ResumoDiarioDTO
        {
            TotalOSAbertas = await _context.OrdensServico
                .CountAsync(os => os.Status == Domain.Enums.StatusOS.Aberta, cancellationToken),

            TotalOSEmAndamento = await _context.OrdensServico
                .CountAsync(os => os.Status == StatusOS.EmAndamento, cancellationToken),

            TotalOSFinalizadasHoje = await _context.OrdensServico
                .CountAsync(os => os.Status == StatusOS.Finalizada && os.DataFinalizacao.HasValue && os.DataFinalizacao.Value.Date == hoje, cancellationToken),

            FaturamentoHoje = await _context.OrdensServico
                .Where(os => os.Status == StatusOS.Finalizada && os.DataFinalizacao.HasValue && os.DataFinalizacao.Value.Date == hoje)
                .SumAsync(os => os.ValorTotal, cancellationToken),
            ProdutosEstoqueBaixo = await _context.Pecas
                .CountAsync(p => p.Estoque <= p.EstoqueMinimo && p.Ativo, cancellationToken)
        };
    }
}