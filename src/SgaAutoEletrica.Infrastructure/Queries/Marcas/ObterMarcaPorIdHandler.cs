using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Marcas.DTOs;
using SgaAutoEletrica.Application.Features.Marcas.Queries;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Queries.Marcas;

public class ObterMarcaPorIdHandler : IRequestHandler<ObterMarcaPorIdQuery, MarcaDTO?>
{
    private readonly AppDbContext _context;

    public ObterMarcaPorIdHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<MarcaDTO?> Handle(ObterMarcaPorIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Marcas
            .AsNoTracking()
            .Where(m => m.Id == request.Id)
            .Select(m => new MarcaDTO
            {
                Id = m.Id,
                Nome = m.Nome,
                Ativo = m.Ativo
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}