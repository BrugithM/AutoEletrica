using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Marcas.Commands;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.Marcas;

public class AtualizarMarcaHandler : IRequestHandler<AtualizarMarcaCommand>
{
    private readonly AppDbContext _context;

    public AtualizarMarcaHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(AtualizarMarcaCommand request, CancellationToken cancellationToken)
    {
        var marca = await _context.Marcas
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken)
            ?? throw new InvalidOperationException("Marca não encontrada.");

        marca.Atualizar(request.Nome);
        await _context.SaveChangesAsync(cancellationToken);
    }
}