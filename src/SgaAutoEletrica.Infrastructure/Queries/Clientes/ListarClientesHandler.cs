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
            var termo = request.TermoBusca.Trim().ToLower();
            query = query.Where(c =>
                c.NomeCompleto.ToLower().Contains(termo) ||
                c.Cpf.Valor.Contains(termo) ||
                c.Telefone.Valor.Contains(termo));
        }

        return await query
            .AsNoTracking()
            .OrderBy(c => c.NomeCompleto)
            .Select(c => new ClienteDTO
            {
                Id = c.Id,
                NomeCompleto = c.NomeCompleto,
                Cpf = c.Cpf.Formatado(),
                Telefone = c.Telefone.Formatado(),
                EnderecoCompleto = c.Endereco != null ? c.Endereco.Completo() : null,
                DataCadastro = c.DataCadastro,
                QuantidadeVeiculos = c.Veiculos.Count
            })
            .ToListAsync(cancellationToken);
    }
}