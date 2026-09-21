using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Servicos.DTOs;
using SgaAutoEletrica.Application.Features.Servicos.Queries;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Queries.Servicos;

public class ListarServicosHandler : IRequestHandler<ListarServicosQuery, List<ServicoDTO>>
{
    private readonly AppDbContext _context;

    public ListarServicosHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ServicoDTO>> Handle(ListarServicosQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Servicos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.TermoBusca))
        {
            var termo = request.TermoBusca.Trim().ToLower();
            query = query.Where(s => s.Nome.ToLower().Contains(termo));
        }

        return await query
            .AsNoTracking()
            .OrderBy(s => s.Nome)
            .Select(s => new ServicoDTO
            {
                Id = s.Id,
                Nome = s.Nome,
                Descricao = s.Descricao,
                PrecoPadrao = s.PrecoPadrao,
                Ativo = s.Ativo
            })
            .ToListAsync(cancellationToken);
    }
}