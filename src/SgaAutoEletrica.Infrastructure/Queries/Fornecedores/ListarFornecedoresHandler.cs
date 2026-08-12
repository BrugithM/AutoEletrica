using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Fornecedores.DTOs;
using SgaAutoEletrica.Application.Features.Fornecedores.Queries;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Queries.Fornecedores;

public class ListarFornecedoresHandler : IRequestHandler<ListarFornecedoresQuery, List<FornecedorDTO>>
{
    private readonly AppDbContext _context;

    public ListarFornecedoresHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<FornecedorDTO>> Handle(ListarFornecedoresQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Fornecedores.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.TermoBusca))
        {
            query = query.Where(f => f.NomeEmpresa.Contains(request.TermoBusca));
        }

        return await query
            .OrderBy(f => f.NomeEmpresa)
            .Select(f => new FornecedorDTO
            {
                Id = f.Id,
                NomeEmpresa = f.NomeEmpresa,
                Cnpj = f.Cnpj.Valor,
                Telefone = f.Telefone != null ? f.Telefone.Valor : null,
                Contato = f.Contato
            })
            .ToListAsync(cancellationToken);
    }
}