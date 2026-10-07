using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Marcas.Commands;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.Marcas;

public class ExcluirMarcaHandler : IRequestHandler<ExcluirMarcaCommand>
{
    private readonly AppDbContext _context;

    public ExcluirMarcaHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ExcluirMarcaCommand request, CancellationToken cancellationToken)
    {
        var marca = await _context.Marcas
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken)
            ?? throw new InvalidOperationException("Marca não encontrada.");

        _context.Marcas.Remove(marca);
        await _context.SaveChangesAsync(cancellationToken);
    }
}