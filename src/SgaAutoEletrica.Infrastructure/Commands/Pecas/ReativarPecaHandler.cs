using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Pecas.Commands;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.Pecas;

public class ReativarPecaHandler : IRequestHandler<ReativarPecaCommand>
{
    private readonly AppDbContext _context;

    public ReativarPecaHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ReativarPecaCommand request, CancellationToken cancellationToken)
    {
        var peca = await _context.Pecas
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new InvalidOperationException("Peça não encontrada.");

        peca.Ativar();
        await _context.SaveChangesAsync(cancellationToken);
    }
}