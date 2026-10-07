using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Marcas.Commands;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.Marcas;

public class ReativarMarcaHandler : IRequestHandler<ReativarMarcaCommand>
{
    private readonly AppDbContext _context;

    public ReativarMarcaHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ReativarMarcaCommand request, CancellationToken cancellationToken)
    {
        var marca = await _context.Marcas
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken)
            ?? throw new InvalidOperationException("Marca não encontrada.");

        marca.Ativar();
        await _context.SaveChangesAsync(cancellationToken);
    }
}