using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Clientes.DTOs;
using SgaAutoEletrica.Application.Features.Clientes.Queries;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Queries.Clientes;

public class ListarClientesHandler : IRequestHandler<ListarClientesQuery, List<ClienteDTO>>
{
    private readonly AppDbContext _context;

    public ListarClientesHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ClienteDTO>> Handle(ListarClientesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Clientes.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.TermoBusca))
        {
            query = query.Where(c => c.NomeCompleto.Contains(request.TermoBusca));
        }

        return await query
            .OrderBy(c => c.NomeCompleto)
            .Select(c => new ClienteDTO
            {
                Id = c.Id,
                NomeCompleto = c.NomeCompleto,
                Cpf = c.Cpf.Valor,
                Telefone = c.Telefone.Valor,
                DataCadastro = c.DataCadastro,
                QuantidadeVeiculos = c.Veiculos.Count
            })
            .ToListAsync(cancellationToken);
    }
}