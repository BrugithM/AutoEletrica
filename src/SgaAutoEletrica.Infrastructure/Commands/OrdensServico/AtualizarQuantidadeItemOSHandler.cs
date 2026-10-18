using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.OrdensServico.Commands;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.OrdensServico;

public class AtualizarQuantidadeItemOSHandler : IRequestHandler<AtualizarQuantidadeItemOSCommand>
{
    private readonly AppDbContext _context;

    public AtualizarQuantidadeItemOSHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(AtualizarQuantidadeItemOSCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.ItensPecaOS
            .Include(i => i.Peca)
            .FirstOrDefaultAsync(i => i.Id == request.ItemId, cancellationToken)
            ?? throw new InvalidOperationException("Item de peça não encontrado.");

        var os = await _context.OrdensServico
            .Include(o => o.ItensPeca)
            .Include(o => o.ItensServico)
            .FirstOrDefaultAsync(o => o.Id == request.OrdemServicoId, cancellationToken)
            ?? throw new InvalidOperationException("OS não encontrada.");

        var diferenca = request.NovaQuantidade - item.Quantidade;

        if (diferenca > 0)
        {
            if (item.Peca.Estoque < diferenca)
                throw new InvalidOperationException(
                    $"Estoque insuficiente. Disponível: {item.Peca.Estoque:0.###}, " +
                    $"necessário adicional: {diferenca:0.###}.");
            item.Peca.DarBaixaEstoque(diferenca);
        }
        else if (diferenca < 0)
        {
            item.Peca.DarEntradaEstoque(Math.Abs(diferenca));
        }

        item.AtualizarQuantidade(request.NovaQuantidade);

        var totalPecas = os.ItensPeca.Sum(i => i.ValorTotal);
        var totalServicos = os.ItensServico.Sum(i => i.PrecoUnitario);
        os.AtualizarTotais(totalPecas, totalServicos);

        await _context.SaveChangesAsync(cancellationToken);
    }
}