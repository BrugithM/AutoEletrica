using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Common.DTOs;
using SgaAutoEletrica.Application.Features.Pecas.DTOs;
using SgaAutoEletrica.Application.Features.Pecas.Queries;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Queries.Pecas;

public class ListarPecasHandler : IRequestHandler<ListarPecasQuery, ListaPaginadaDTO<PecaDTO>>
{
    private readonly AppDbContext _context;

    public ListarPecasHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ListaPaginadaDTO<PecaDTO>> Handle(ListarPecasQuery request, CancellationToken cancellationToken)
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

        if (!string.IsNullOrWhiteSpace(request.Nome))
        {
            var termo = request.Nome.Trim().ToLower();
            query = query.Where(p => p.Nome.ToLower().Contains(termo));
        }

        if (!string.IsNullOrWhiteSpace(request.CodigoPeca))
        {
            var termo = request.CodigoPeca.Trim().ToLower();
            query = query.Where(p => p.CodigoPeca != null && p.CodigoPeca.ToLower().Contains(termo));
        }

        if (!string.IsNullOrWhiteSpace(request.CodigoBarras))
        {
            var termo = request.CodigoBarras.Trim();
            query = query.Where(p => p.CodigoBarras != null && p.CodigoBarras.Valor.Contains(termo));
        }

        if (!string.IsNullOrWhiteSpace(request.IdPeca))
        {
            var termo = request.IdPeca.Trim().ToLower();
            query = query.Where(p => p.IdPeca.ToLower().Contains(termo));
        }

        if (!string.IsNullOrWhiteSpace(request.Marca))
        {
            var termo = request.Marca.Trim().ToLower();
            query = query.Where(p => p.Marca.ToLower().Contains(termo));
        }

        if (request.CategoriaId.HasValue && request.CategoriaId.Value != Guid.Empty)
            query = query.Where(p => p.CategoriaId == request.CategoriaId.Value);

        if (request.FornecedorId.HasValue && request.FornecedorId.Value != Guid.Empty)
            query = query.Where(p => p.FornecedorId == request.FornecedorId.Value);

        if (request.ApenasEstoqueBaixo)
            query = query.Where(p => p.Estoque <= p.EstoqueMinimo);

        if (request.Ativo.HasValue)
            query = query.Where(p => p.Ativo == request.Ativo.Value);

        var totalItens = await query.CountAsync(cancellationToken);

        var itens = await query
            .OrderBy(p => p.Nome)
            .Skip((request.Pagina - 1) * request.TamanhoPagina)
            .Take(request.TamanhoPagina)
            .Select(p => new PecaDTO
            {
                Id = p.Id,
                IdPeca = p.IdPeca,
                CodigoPeca = p.CodigoPeca,
                CodigoBarras = p.CodigoBarras != null ? p.CodigoBarras.Valor : null,
                Nome = p.Nome,
                Descricao = p.Descricao,
                Marca = p.Marca,
                CategoriaId = p.CategoriaId,
                CategoriaNome = p.CategoriaPeca != null ? p.CategoriaPeca.Nome : null,
                FornecedorId = p.FornecedorId,
                FornecedorNome = p.Fornecedor != null ? p.Fornecedor.NomeEmpresa : null,
                FornecedorCnpj = p.Fornecedor != null ? p.Fornecedor.Cnpj.Formatado() : null,
                FornecedorTelefone = p.Fornecedor != null && p.Fornecedor.Telefone != null
                    ? p.Fornecedor.Telefone.Formatado() : null,
                FornecedorContato = p.Fornecedor != null ? p.Fornecedor.Contato : null,
                ValorCusto = p.ValorCusto,
                ValorVenda = p.ValorVenda,
                Estoque = p.Estoque,
                EstoqueMinimo = p.EstoqueMinimo,
                Ativo = p.Ativo,
                MargemLucro = p.CalcularMargemLucroPercentual(),
                EstoqueBaixo = p.Estoque <= p.EstoqueMinimo
            })
            .ToListAsync(cancellationToken);

        return new ListaPaginadaDTO<PecaDTO>
        {
            Itens = itens,
            PaginaAtual = request.Pagina,
            TamanhoPagina = request.TamanhoPagina,
            TotalItens = totalItens
        };
    }
}