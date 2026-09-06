using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Fornecedores.Commands;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.Fornecedores;

public class ExcluirFornecedorHandler : IRequestHandler<ExcluirFornecedorCommand>
{
    private readonly AppDbContext _context;

    public ExcluirFornecedorHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ExcluirFornecedorCommand request, CancellationToken cancellationToken)
    {
        var fornecedor = await _context.Fornecedores
            .FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken)
            ?? throw new InvalidOperationException("Fornecedor não encontrado.");

        _context.Fornecedores.Remove(fornecedor);
        await _context.SaveChangesAsync(cancellationToken);
    }
}