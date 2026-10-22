using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Fornecedores.Commands;
using SgaAutoEletrica.Domain.Entities;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.Fornecedores;

public class AtualizarNotaFiscalEntradaHandler : IRequestHandler<AtualizarNotaFiscalEntradaCommand>
{
    private readonly AppDbContext _context;

    public AtualizarNotaFiscalEntradaHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(AtualizarNotaFiscalEntradaCommand request, CancellationToken cancellationToken)
    {
        var nfOriginal = await _context.NotasFiscaisEntrada
            .Include(n => n.Itens)
            .FirstOrDefaultAsync(n => n.Id == request.Id, cancellationToken)
            ?? throw new InvalidOperationException("NF não encontrada.");

        foreach (var itemAntigo in nfOriginal.Itens.ToList())
        {
            var peca = await _context.Pecas.FindAsync([itemAntigo.PecaId], cancellationToken);
            if (peca != null)
            {
                peca.DarBaixaEstoque(itemAntigo.Quantidade);
            }
        }

        _context.NotasFiscaisEntrada.Remove(nfOriginal);
        await _context.SaveChangesAsync(cancellationToken);

        _context.ChangeTracker.Clear();

        var nfNova = new NotaFiscalEntrada(
            request.Numero,
            request.FornecedorId,
            request.DataEntrada,
            request.Observacao);

        typeof(NotaFiscalEntrada)
            .GetProperty("Id")?
            .SetValue(nfNova, request.Id);

        foreach (var itemTela in request.Itens)
        {
            var peca = await _context.Pecas.FindAsync([itemTela.PecaId], cancellationToken)
                ?? throw new InvalidOperationException($"Peça {itemTela.PecaId} não encontrada.");

            nfNova.AdicionarItem(itemTela.PecaId, itemTela.Quantidade, itemTela.ValorUnitario);
            peca.DarEntradaEstoque(itemTela.Quantidade);
        }

        nfNova.RecalcularTotal();

        await _context.NotasFiscaisEntrada.AddAsync(nfNova, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}