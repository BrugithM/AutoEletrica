using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Pecas.Commands;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.Pecas;

public class ExcluirPecaHandler : IRequestHandler<ExcluirPecaCommand>
{
    private readonly AppDbContext _context;

    public ExcluirPecaHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ExcluirPecaCommand request, CancellationToken cancellationToken)
    {
        var peca = await _context.Pecas
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new InvalidOperationException("Peça não encontrada.");

        _context.Pecas.Remove(peca);
        await _context.SaveChangesAsync(cancellationToken);
    }
}