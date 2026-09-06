using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.CategoriasPeca.Commands;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.CategoriasPeca;

public class ExcluirCategoriaPecaHandler : IRequestHandler<ExcluirCategoriaPecaCommand>
{
    private readonly AppDbContext _context;

    public ExcluirCategoriaPecaHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ExcluirCategoriaPecaCommand request, CancellationToken cancellationToken)
    {
        var categoria = await _context.CategoriasPecas
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
            ?? throw new InvalidOperationException("Categoria não encontrada.");

        _context.CategoriasPecas.Remove(categoria);
        await _context.SaveChangesAsync(cancellationToken);
    }
}