using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Pecas.Commands;
using SgaAutoEletrica.Domain.Entities;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.Pecas;

public class CriarPecaHandler : IRequestHandler<CriarPecaCommand, Guid>
{
    private readonly AppDbContext _context;

    public CriarPecaHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CriarPecaCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Verifica se a categoria existe
            if (request.CategoriaId.HasValue)
            {
                var catExiste = await _context.CategoriasPecas
                    .AnyAsync(c => c.Id == request.CategoriaId.Value, cancellationToken);
                if (!catExiste)
                    throw new InvalidOperationException($"Categoria {request.CategoriaId} não existe.");
            }

            // Verifica se o fornecedor existe
            if (request.FornecedorId.HasValue)
            {
                var fornExiste = await _context.Fornecedores
                    .AnyAsync(f => f.Id == request.FornecedorId.Value, cancellationToken);
                if (!fornExiste)
                    throw new InvalidOperationException($"Fornecedor {request.FornecedorId} não existe.");
            }

            var peca = new Peca(
                request.IdPeca,
                request.Nome,
                request.Descricao,
                request.Marca,
                request.ValorCusto,
                request.ValorVenda,
                request.Imposto,
                request.EstoqueInicial,
                request.EstoqueMinimo,
                request.CodigoPeca,
                request.CodigoBarras,
                request.CategoriaId,
                request.FornecedorId);

            await _context.Pecas.AddAsync(peca, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return peca.Id;
        }
        catch (Exception ex)
        {
            var inner = ex;
            while (inner.InnerException != null)
                inner = inner.InnerException;

            throw new Exception($"ERRO DETALHADO: {inner.Message}", ex);
        }
    }
}