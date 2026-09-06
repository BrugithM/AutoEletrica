using MediatR;
using SgaAutoEletrica.Application.Features.Fornecedores.Commands;
using SgaAutoEletrica.Domain.Entities;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.Fornecedores;

public class CriarFornecedorHandler : IRequestHandler<CriarFornecedorCommand, Guid>
{
    private readonly AppDbContext _context;

    public CriarFornecedorHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CriarFornecedorCommand request, CancellationToken cancellationToken)
    {
        var fornecedor = new Fornecedor(request.NomeEmpresa, request.Cnpj);

        if (!string.IsNullOrWhiteSpace(request.Telefone) || !string.IsNullOrWhiteSpace(request.Contato))
            fornecedor.AtualizarDados(request.NomeEmpresa, request.Telefone, request.Contato);

        await _context.Fornecedores.AddAsync(fornecedor, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return fornecedor.Id;
    }
}