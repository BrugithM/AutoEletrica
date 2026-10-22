using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.MarcasVeiculo.DTOs;
using SgaAutoEletrica.Application.Features.MarcasVeiculo.Queries;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Queries.MarcasVeiculo;
public class ListarMarcasVeiculoHandler : IRequestHandler<ListarMarcasVeiculoQuery, List<MarcaVeiculoDTO>>
{
    private readonly AppDbContext _context;
    public ListarMarcasVeiculoHandler(AppDbContext context) => _context = context;

    public async Task<List<MarcaVeiculoDTO>> Handle(ListarMarcasVeiculoQuery request, CancellationToken cancellationToken)
    {
        var query = _context.MarcasVeiculo.AsNoTracking();
        if (request.Ativo.HasValue)
            query = query.Where(m => m.Ativo == request.Ativo.Value);

        return await query.OrderBy(m => m.Nome)
            .Select(m => new MarcaVeiculoDTO { Id = m.Id, Nome = m.Nome, Ativo = m.Ativo })
            .ToListAsync(cancellationToken);
    }
}