using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Fornecedores.DTOs;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Application.Features.Fornecedores.Queries;

public class ObterFornecedorPorIdHandler : IRequestHandler<ObterFornecedorPorIdQuery, FornecedorDTO?>
{
    private readonly AppDbContext _context;

    public ObterFornecedorPorIdHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<FornecedorDTO?> Handle(ObterFornecedorPorIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Fornecedores
            .Where(f => f.Id == request.Id)
            .Select(f => new FornecedorDTO
            {
                Id = f.Id,
                NomeEmpresa = f.NomeEmpresa,
                Cnpj = f.Cnpj.Valor,
                Telefone = f.Telefone != null ? f.Telefone.Valor : null,
                Contato = f.Contato
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}