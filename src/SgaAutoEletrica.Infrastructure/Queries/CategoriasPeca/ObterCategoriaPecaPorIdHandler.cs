using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.CategoriasPeca.DTOs;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Application.Features.CategoriasPeca.Queries;

public class ObterCategoriaPecaPorIdHandler : IRequestHandler<ObterCategoriaPecaPorIdQuery, CategoriaPecaDTO?>
{
    private readonly AppDbContext _context;

    public ObterCategoriaPecaPorIdHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CategoriaPecaDTO?> Handle(ObterCategoriaPecaPorIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.CategoriasPecas
            .Where(c => c.Id == request.Id)
            .Select(c => new CategoriaPecaDTO
            {
                Id = c.Id,
                Nome = c.Nome,
                Descricao = c.Descricao
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}