using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Pecas.Commands;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.Pecas;

public class AtualizarPecaHandler : IRequestHandler<AtualizarPecaCommand>
{
    private readonly AppDbContext _context;

    public AtualizarPecaHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(AtualizarPecaCommand request, CancellationToken cancellationToken)
    {
        var peca = await _context.Pecas
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new InvalidOperationException("Peça não encontrada.");

        peca.AtualizarDados(
            request.Nome,
            request.Descricao,
            request.ValorCusto,
            request.ValorVenda,
            request.CodigoPeca,
            request.MarcaId,
            request.CategoriaId,
            request.FornecedorId,
            request.EstoqueMinimo);

        await _context.SaveChangesAsync(cancellationToken);
    }
}