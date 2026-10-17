using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Clientes.DTOs;
using SgaAutoEletrica.Application.Features.Clientes.Queries;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Queries.Clientes;

public class ObterClientePorIdHandler : IRequestHandler<ObterClientePorIdQuery, ClienteDTO?>
{
    private readonly AppDbContext _context;

    public ObterClientePorIdHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ClienteDTO?> Handle(ObterClientePorIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Clientes
            .AsNoTracking()
            .Where(c => c.Id == request.Id)
            .Select(c => new ClienteDTO
            {
                Id = c.Id,
                NomeCompleto = c.NomeCompleto,
                CpfCnpj = c.CpfCnpj != null ? c.CpfCnpj.Formatado() : null,
                Telefone = c.Telefone != null ? c.Telefone.Formatado() : null,
                EnderecoCompleto = c.Endereco != null ? c.Endereco.Completo() : null,
                DataCadastro = c.DataCadastro,
                QuantidadeVeiculos = c.Veiculos.Count
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}