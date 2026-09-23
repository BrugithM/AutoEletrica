using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Fornecedores.Queries;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Queries.Fornecedores;

public class VerificarNFExistenteHandler : IRequestHandler<VerificarNFExistenteQuery, bool>
{
    private readonly AppDbContext _context;

    public VerificarNFExistenteHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(VerificarNFExistenteQuery request, CancellationToken cancellationToken)
    {
        return await _context.NotasFiscaisEntrada
            .AnyAsync(nf => nf.Numero == request.Numero && nf.FornecedorId == request.FornecedorId, cancellationToken);
    }
}