using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Clientes.DTOs;
using SgaAutoEletrica.Application.Features.Clientes.Queries;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Queries.Clientes;

public class ObterClienteParaEdicaoHandler : IRequestHandler<ObterClienteParaEdicaoQuery, ClienteEdicaoDTO?>
{
    private readonly AppDbContext _context;

    public ObterClienteParaEdicaoHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ClienteEdicaoDTO?> Handle(ObterClienteParaEdicaoQuery request, CancellationToken cancellationToken)
    {
        return await _context.Clientes
            .AsNoTracking()
            .Where(c => c.Id == request.Id)
            .Select(c => new ClienteEdicaoDTO
            {
                Id = c.Id,
                NomeCompleto = c.NomeCompleto,
                Cpf = c.Cpf.Formatado(),
                Telefone = c.Telefone.Formatado(),
                Logradouro = c.Endereco != null ? c.Endereco.Logradouro : null,
                Numero = c.Endereco != null ? c.Endereco.Numero : null,
                Complemento = c.Endereco != null ? c.Endereco.Complemento : null,
                Bairro = c.Endereco != null ? c.Endereco.Bairro : null,
                Cidade = c.Endereco != null ? c.Endereco.Cidade : null,
                Estado = c.Endereco != null ? c.Endereco.Estado : null,
                Cep = c.Endereco != null ? c.Endereco.Cep : null
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}