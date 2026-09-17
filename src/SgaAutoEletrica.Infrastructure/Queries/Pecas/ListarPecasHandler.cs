using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Pecas.DTOs;
using SgaAutoEletrica.Application.Features.Pecas.Queries;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Queries.Pecas;

public class ListarPecasHandler : IRequestHandler<ListarPecasQuery, List<PecaDTO>>
{
    private readonly AppDbContext _context;

    public ListarPecasHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<PecaDTO>> Handle(ListarPecasQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Pecas
            .Include(p => p.CategoriaPeca)
            .Include(p => p.Fornecedor)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.TermoBusca))
        {
            var termo = request.TermoBusca.Trim().ToLower();
            query = query.Where(p =>
                p.Nome.ToLower().Contains(termo) ||
                p.IdPeca.ToLower().Contains(termo) ||
                (p.CodigoPeca != null && p.CodigoPeca.ToLower().Contains(termo)) ||
                (p.CodigoBarras != null && p.CodigoBarras.Valor.Contains(termo)) ||
                p.Marca.ToLower().Contains(termo));
        }

        if (request.CategoriaId.HasValue)
            query = query.Where(p => p.CategoriaId == request.CategoriaId.Value);

        if (request.Ativo.HasValue)
            query = query.Where(p => p.Ativo == request.Ativo.Value);

        return await query
            .OrderBy(p => p.Nome)
            .Select(p => new PecaDTO
            {
                Id = p.Id,
                IdPeca = p.IdPeca,
                CodigoPeca = p.CodigoPeca,
                CodigoBarras = p.CodigoBarras != null ? p.CodigoBarras.Valor : null,
                Nome = p.Nome,
                Descricao = p.Descricao,
                Marca = p.Marca,
                CategoriaNome = p.CategoriaPeca != null ? p.CategoriaPeca.Nome : null,
                FornecedorId = p.FornecedorId,
                FornecedorNome = p.Fornecedor != null ? p.Fornecedor.NomeEmpresa : null,
                FornecedorCnpj = p.Fornecedor != null ? p.Fornecedor.Cnpj.Formatado() : null,
                FornecedorTelefone = p.Fornecedor != null && p.Fornecedor.Telefone != null 
                    ? p.Fornecedor.Telefone.Formatado() : null,
                FornecedorContato = p.Fornecedor != null ? p.Fornecedor.Contato : null,
                ValorCusto = p.ValorCusto,
                ValorVenda = p.ValorVenda,
                Imposto = p.Imposto,
                Estoque = p.Estoque,
                EstoqueMinimo = p.EstoqueMinimo,
                Ativo = p.Ativo,
                MargemLucro = p.CalcularMargemLucroPercentual(),
                PrecoComImposto = p.CalcularPrecoComImposto(),
                EstoqueBaixo = p.Estoque <= p.EstoqueMinimo
            })
            .ToListAsync(cancellationToken);
    }
}