using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Pecas.Commands;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.Pecas;

public class AtualizarPrecoVendaPecaHandler : IRequestHandler<AtualizarPrecoVendaPecaCommand>
{
    private readonly AppDbContext _context;

    public AtualizarPrecoVendaPecaHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(AtualizarPrecoVendaPecaCommand request, CancellationToken cancellationToken)
    {
        var peca = await _context.Pecas
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new InvalidOperationException("Peça não encontrada.");

        peca.AtualizarValorVenda(request.NovoValorVenda);

        await _context.SaveChangesAsync(cancellationToken);
    }
}