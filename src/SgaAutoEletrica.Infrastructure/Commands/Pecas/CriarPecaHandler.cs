using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Pecas.Commands;
using SgaAutoEletrica.Domain.Entities;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.Pecas;

public class CriarPecaHandler : IRequestHandler<CriarPecaCommand, int>
{
    private readonly AppDbContext _context;

    public CriarPecaHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CriarPecaCommand request, CancellationToken cancellationToken)
    {
        if (request.CategoriaId.HasValue)
        {
            var catExiste = await _context.CategoriasPecas
                .AnyAsync(c => c.Id == request.CategoriaId.Value, cancellationToken);
            if (!catExiste)
                throw new InvalidOperationException("Categoria selecionada não existe.");
        }

        if (request.FornecedorId.HasValue)
        {
            var fornExiste = await _context.Fornecedores
                .AnyAsync(f => f.Id == request.FornecedorId.Value, cancellationToken);
            if (!fornExiste)
                throw new InvalidOperationException("Fornecedor selecionado não existe.");
        }

        if (request.MarcaId.HasValue)
        {
            var marcaExiste = await _context.Marcas
                .AnyAsync(m => m.Id == request.MarcaId.Value, cancellationToken);
            if (!marcaExiste)
                throw new InvalidOperationException("Marca selecionada não existe.");
        }

        var peca = new Peca(
            request.Nome,
            request.Descricao,
            request.ValorCusto,
            request.ValorVenda,
            request.EstoqueInicial,
            request.EstoqueMinimo,
            request.CodigoPeca,
            request.CodigoBarras,
            request.MarcaId,
            request.CategoriaId,
            request.FornecedorId);

        await _context.Pecas.AddAsync(peca, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return peca.Id;
    }
}