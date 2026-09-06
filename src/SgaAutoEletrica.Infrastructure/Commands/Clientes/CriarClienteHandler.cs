using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Clientes.Commands;
using SgaAutoEletrica.Domain.Entities;
using SgaAutoEletrica.Domain.ValueObjects;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.Clientes;

public class CriarClienteHandler : IRequestHandler<CriarClienteCommand, Guid>
{
    private readonly AppDbContext _context;

    public CriarClienteHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CriarClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = new Cliente(request.NomeCompleto, request.Cpf, request.Telefone);

        if (!string.IsNullOrWhiteSpace(request.Logradouro) &&
            !string.IsNullOrWhiteSpace(request.Bairro) &&
            !string.IsNullOrWhiteSpace(request.Cidade) &&
            !string.IsNullOrWhiteSpace(request.Estado) &&
            !string.IsNullOrWhiteSpace(request.Cep))
        {
            var endereco = new Endereco(
                request.Logradouro,
                request.Bairro,
                request.Cidade,
                request.Estado,
                request.Cep,
                request.Numero,
                request.Complemento);
            cliente.AdicionarEndereco(endereco);
        }

        await _context.Clientes.AddAsync(cliente, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return cliente.Id;
    }
}