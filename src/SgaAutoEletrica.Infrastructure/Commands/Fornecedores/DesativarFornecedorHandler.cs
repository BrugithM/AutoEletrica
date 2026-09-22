using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Fornecedores.Commands;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.Fornecedores;

public class DesativarFornecedorHandler : IRequestHandler<DesativarFornecedorCommand>
{
    private readonly AppDbContext _context;

    public DesativarFornecedorHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DesativarFornecedorCommand request, CancellationToken cancellationToken)
    {
        var fornecedor = await _context.Fornecedores
            .FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken)
            ?? throw new InvalidOperationException("Fornecedor não encontrado.");

        fornecedor.Desativar();
        await _context.SaveChangesAsync(cancellationToken);
    }
}