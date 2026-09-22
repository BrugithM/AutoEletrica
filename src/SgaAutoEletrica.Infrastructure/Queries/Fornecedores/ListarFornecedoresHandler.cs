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
        var query = _context.Fornecedores
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.TermoBusca))
        {
            var termo = request.TermoBusca.Trim().ToLower();
            query = query.Where(f =>
                f.NomeEmpresa.ToLower().Contains(termo) ||
                f.Cnpj.Valor.Contains(termo) ||
                (f.Contato != null && f.Contato.ToLower().Contains(termo)));
        }

        if (request.Ativo.HasValue)
            query = query.Where(f => f.Ativo == request.Ativo.Value);

        return await query
            .OrderBy(f => f.NomeEmpresa)
            .Select(f => new FornecedorDTO
            {
                Id = f.Id,
                NomeEmpresa = f.NomeEmpresa,
                Cnpj = f.Cnpj.Formatado(),
                Telefone = f.Telefone != null ? f.Telefone.Formatado() : null,
                Contato = f.Contato,
                Ativo = f.Ativo
            })
            .ToListAsync(cancellationToken);
    }
}