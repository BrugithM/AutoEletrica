using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.CategoriasPeca.Commands;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.CategoriasPeca;

public class AtualizarCategoriaPecaHandler : IRequestHandler<AtualizarCategoriaPecaCommand>
{
    private readonly AppDbContext _context;

    public AtualizarCategoriaPecaHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(AtualizarCategoriaPecaCommand request, CancellationToken cancellationToken)
    {
        var categoria = await _context.CategoriasPecas
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
            ?? throw new InvalidOperationException("Categoria não encontrada.");

        categoria.Atualizar(request.Nome, request.Descricao);

        await _context.SaveChangesAsync(cancellationToken);
    }
}