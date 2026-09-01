using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.OrdensServico.Commands;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.OrdensServico;

public class RemoverItemServicoOSHandler : IRequestHandler<RemoverItemServicoOSCommand>
{
    private readonly AppDbContext _context;

    public RemoverItemServicoOSHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(RemoverItemServicoOSCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.ItensServicoOS
            .FirstOrDefaultAsync(i => i.Id == request.ItemId, cancellationToken)
            ?? throw new InvalidOperationException("Item não encontrado.");

        _context.ItensServicoOS.Remove(item);

        var os = await _context.OrdensServico
            .FirstOrDefaultAsync(o => o.Id == request.OrdemServicoId, cancellationToken)
            ?? throw new InvalidOperationException("OS não encontrada.");

        var totalPecas = await _context.ItensPecaOS
            .Where(i => i.OrdemServicoId == request.OrdemServicoId)
            .SumAsync(i => i.ValorTotal, cancellationToken);

        var totalServicos = await _context.ItensServicoOS
            .Where(i => i.OrdemServicoId == request.OrdemServicoId)
            .SumAsync(i => i.PrecoUnitario, cancellationToken);

        os.AtualizarTotais(totalPecas, totalServicos);

        await _context.SaveChangesAsync(cancellationToken);
    }
}