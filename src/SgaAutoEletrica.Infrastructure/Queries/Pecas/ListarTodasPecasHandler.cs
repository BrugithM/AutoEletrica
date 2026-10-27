using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Pecas.DTOs;
using SgaAutoEletrica.Application.Features.Pecas.Queries;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Queries.Pecas;

public class ListarTodasPecasHandler : IRequestHandler<ListarTodasPecasQuery, List<PecaDTO>>
{
    private readonly AppDbContext _context;

    public ListarTodasPecasHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<PecaDTO>> Handle(ListarTodasPecasQuery request, CancellationToken cancellationToken)
    {
        return await _context.Pecas
            .AsNoTracking()
            .Where(p => p.Ativo)
            .OrderBy(p => p.Nome)
            .Select(p => new PecaDTO
            {
                Id = p.Id,
                CodigoPeca = p.CodigoPeca,
                CodigoBarras = p.CodigoBarras != null ? p.CodigoBarras.Valor : null,
                Nome = p.Nome,
                Descricao = p.Descricao,
                MarcaId = p.MarcaId,
                MarcaNome = p.Marca != null ? p.Marca.Nome : null,
                CategoriaId = p.CategoriaId,
                CategoriaNome = p.CategoriaPeca != null ? p.CategoriaPeca.Nome : null,
                FornecedorId = p.FornecedorId,
                FornecedorNome = p.Fornecedor != null ? p.Fornecedor.NomeEmpresa : null,
                ValorCusto = p.ValorCusto, 
                ValorVenda = p.ValorVenda, 
                MarkupPercentual = p.MarkupPercentual,
                MargemLucro = p.CalcularMargemLucroPercentual(),
                Estoque = p.Estoque,
                EstoqueMinimo = p.EstoqueMinimo,
                Ativo = p.Ativo,
                EstoqueBaixo = p.Estoque <= p.EstoqueMinimo
            })
            .ToListAsync(cancellationToken);
    }
}