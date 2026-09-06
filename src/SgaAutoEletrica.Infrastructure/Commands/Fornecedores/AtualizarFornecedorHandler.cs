using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Fornecedores.Commands;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.Fornecedores;

public class AtualizarFornecedorHandler : IRequestHandler<AtualizarFornecedorCommand>
{
    private readonly AppDbContext _context;

    public AtualizarFornecedorHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(AtualizarFornecedorCommand request, CancellationToken cancellationToken)
    {
        var fornecedor = await _context.Fornecedores
            .FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken)
            ?? throw new InvalidOperationException("Fornecedor não encontrado.");

        fornecedor.AtualizarDados(request.NomeEmpresa, request.Telefone, request.Contato);

        await _context.SaveChangesAsync(cancellationToken);
    }
}