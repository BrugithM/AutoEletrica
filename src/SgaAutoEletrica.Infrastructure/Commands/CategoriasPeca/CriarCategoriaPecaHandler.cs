using MediatR;
using SgaAutoEletrica.Application.Features.CategoriasPeca.Commands;
using SgaAutoEletrica.Domain.Entities;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.CategoriasPeca;

public class CriarCategoriaPecaHandler : IRequestHandler<CriarCategoriaPecaCommand, Guid>
{
    private readonly AppDbContext _context;

    public CriarCategoriaPecaHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CriarCategoriaPecaCommand request, CancellationToken cancellationToken)
    {
        var categoria = new CategoriaPeca(request.Nome, request.Descricao);

        await _context.CategoriasPecas.AddAsync(categoria, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return categoria.Id;
    }
}