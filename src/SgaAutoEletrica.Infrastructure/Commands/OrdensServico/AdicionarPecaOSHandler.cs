using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.OrdensServico.Commands;
using SgaAutoEletrica.Domain.Entities;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.OrdensServico;

public class AdicionarPecaOSHandler : IRequestHandler<AdicionarPecaOSCommand>
{
    private readonly AppDbContext _context;

    public AdicionarPecaOSHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(AdicionarPecaOSCommand request, CancellationToken cancellationToken)
    {
        var peca = await _context.Pecas
            .FirstOrDefaultAsync(p => p.Id == request.PecaId, cancellationToken)
            ?? throw new InvalidOperationException("Peça não encontrada.");

        peca.DarBaixaEstoque(request.Quantidade);

        var os = await _context.OrdensServico
            .FirstOrDefaultAsync(o => o.Id == request.OrdemServicoId, cancellationToken)
            ?? throw new InvalidOperationException("OS não encontrada.");

        var novoItem = new ItemPecaOS(request.PecaId, request.Quantidade, peca.ValorVenda);
        novoItem.DefinirOrdemServico(request.OrdemServicoId);
        _context.ItensPecaOS.Add(novoItem);

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