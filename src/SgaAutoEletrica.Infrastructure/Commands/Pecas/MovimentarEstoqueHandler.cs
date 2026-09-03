using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Pecas.Commands;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.Pecas;

public class MovimentarEstoqueHandler : IRequestHandler<MovimentarEstoqueCommand>
{
    private readonly AppDbContext _context;

    public MovimentarEstoqueHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(MovimentarEstoqueCommand request, CancellationToken cancellationToken)
    {
        var peca = await _context.Pecas
            .FirstOrDefaultAsync(p => p.Id == request.PecaId, cancellationToken)
            ?? throw new InvalidOperationException("Peça não encontrada.");

        if (request.Entrada)
            peca.DarEntradaEstoque(request.Quantidade);
        else
            peca.DarBaixaEstoque(request.Quantidade);

        await _context.SaveChangesAsync(cancellationToken);
    }
}