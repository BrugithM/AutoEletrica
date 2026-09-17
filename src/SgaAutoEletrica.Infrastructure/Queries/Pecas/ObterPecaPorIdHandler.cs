using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Pecas.DTOs;
using SgaAutoEletrica.Application.Features.Pecas.Queries;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Queries.Pecas;

public class ObterPecaPorIdHandler : IRequestHandler<ObterPecaPorIdQuery, PecaDTO?>
{
    private readonly AppDbContext _context;

    public ObterPecaPorIdHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PecaDTO?> Handle(ObterPecaPorIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Pecas
            .Include(p => p.CategoriaPeca)
            .Include(p => p.Fornecedor)
            .AsNoTracking()
            .Where(p => p.Id == request.Id)
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
                Imposto = p.Imposto,
                Estoque = p.Estoque,
                EstoqueMinimo = p.EstoqueMinimo,
                Ativo = p.Ativo,
                MargemLucro = p.CalcularMargemLucroPercentual(),
                PrecoComImposto = p.CalcularPrecoComImposto(),
                EstoqueBaixo = p.Estoque <= p.EstoqueMinimo
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}