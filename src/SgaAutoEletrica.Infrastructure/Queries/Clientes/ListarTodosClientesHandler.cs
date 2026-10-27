using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Clientes.DTOs;
using SgaAutoEletrica.Application.Features.Clientes.Queries;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Queries.Clientes;

public class ListarTodosClientesHandler : IRequestHandler<ListarTodosClientesQuery, List<ClienteResumoDTO>>
{
    private readonly AppDbContext _context;

    public ListarTodosClientesHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ClienteResumoDTO>> Handle(ListarTodosClientesQuery request, CancellationToken cancellationToken)
    {
        return await _context.Clientes
            .AsNoTracking()
            .Where(c => c.Ativo)
            .OrderBy(c => c.NomeCompleto)
            .Select(c => new ClienteResumoDTO
            {
                Id = c.Id,
                NomeCompleto = c.NomeCompleto,
                Telefone = c.Telefone != null ? c.Telefone.Formatado() : null
            })
            .ToListAsync(cancellationToken);
    }
}