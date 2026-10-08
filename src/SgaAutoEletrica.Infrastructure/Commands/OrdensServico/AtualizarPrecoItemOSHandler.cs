using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.OrdensServico.Commands;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.OrdensServico;

public class AtualizarPrecoItemOSHandler : IRequestHandler<AtualizarPrecoItemOSCommand>
{
    private readonly AppDbContext _context;

    public AtualizarPrecoItemOSHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(AtualizarPrecoItemOSCommand request, CancellationToken cancellationToken)
    {
        if (request.NovoPreco < 0)
            throw new InvalidOperationException("Preço não pode ser negativo.");

        if (request.EhPeca)
        {
            var item = await _context.ItensPecaOS
                .FirstOrDefaultAsync(i => i.Id == request.ItemId, cancellationToken)
                ?? throw new InvalidOperationException("Item de peça não encontrado.");

            item.AtualizarPrecoUnitario(request.NovoPreco);
        }
        else
        {
            var item = await _context.ItensServicoOS
                .FirstOrDefaultAsync(i => i.Id == request.ItemId, cancellationToken)
                ?? throw new InvalidOperationException("Item de serviço não encontrado.");

            item.AtualizarPrecoUnitario(request.NovoPreco);
        }

        var os = await _context.OrdensServico
            .Include(o => o.ItensPeca)
            .Include(o => o.ItensServico)
            .FirstOrDefaultAsync(o => o.Id == request.OrdemServicoId, cancellationToken)
            ?? throw new InvalidOperationException("OS não encontrada.");

        var totalPecas = os.ItensPeca.Sum(i => i.ValorTotal);
        var totalServicos = os.ItensServico.Sum(i => i.PrecoUnitario);
        os.AtualizarTotais(totalPecas, totalServicos);

        await _context.SaveChangesAsync(cancellationToken);
    }
}