using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Fornecedores.Commands;
using SgaAutoEletrica.Domain.Entities;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.Fornecedores;

public class CriarNotaFiscalEntradaHandler : IRequestHandler<CriarNotaFiscalEntradaCommand, Guid>
{
    private readonly AppDbContext _context;

    public CriarNotaFiscalEntradaHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CriarNotaFiscalEntradaCommand request, CancellationToken cancellationToken)
    {
        var fornecedor = await _context.Fornecedores
            .FirstOrDefaultAsync(f => f.Id == request.FornecedorId, cancellationToken)
            ?? throw new InvalidOperationException("Fornecedor não encontrado.");

        var nf = new NotaFiscalEntrada(request.Numero, request.FornecedorId, request.Observacao);

        foreach (var item in request.Itens)
        {
            var peca = await _context.Pecas.FindAsync([item.PecaId], cancellationToken)
                ?? throw new InvalidOperationException($"Peça {item.PecaId} não encontrada.");

            nf.AdicionarItem(item.PecaId, item.Quantidade, item.ValorUnitario);
        }

        var pecasParaEstoque = nf.Finalizar();

        await _context.NotasFiscaisEntrada.AddAsync(nf, cancellationToken);

        foreach (var (pecaId, quantidade) in pecasParaEstoque)
        {
            var peca = await _context.Pecas.FindAsync([pecaId], cancellationToken);
            peca?.DarEntradaEstoque(quantidade);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return nf.Id;
    }
}