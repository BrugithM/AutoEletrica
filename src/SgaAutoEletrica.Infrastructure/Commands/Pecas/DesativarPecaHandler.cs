using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Pecas.Commands;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.Pecas;

public class DesativarPecaHandler : IRequestHandler<DesativarPecaCommand>
{
    private readonly AppDbContext _context;

    public DesativarPecaHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DesativarPecaCommand request, CancellationToken cancellationToken)
    {
        var peca = await _context.Pecas
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new InvalidOperationException("Peça não encontrada.");

        peca.Desativar();
        await _context.SaveChangesAsync(cancellationToken);
    }
}