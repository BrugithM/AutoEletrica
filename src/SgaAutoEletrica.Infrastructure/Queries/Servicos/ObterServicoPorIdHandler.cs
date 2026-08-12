using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Servicos.DTOs;
using SgaAutoEletrica.Application.Features.Servicos.Queries;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Queries.Servicos;

public class ObterServicoPorIdHandler : IRequestHandler<ObterServicoPorIdQuery, ServicoDTO?>
{
    private readonly AppDbContext _context;

    public ObterServicoPorIdHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ServicoDTO?> Handle(ObterServicoPorIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Servicos
            .Where(s => s.Id == request.Id)
            .Select(s => new ServicoDTO
            {
                Id = s.Id,
                Nome = s.Nome,
                Descricao = s.Descricao,
                PrecoPadrao = s.PrecoPadrao,
                Ativo = s.Ativo
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}