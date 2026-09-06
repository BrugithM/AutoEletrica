using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Clientes.Commands;
using SgaAutoEletrica.Domain.ValueObjects;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.Clientes;

public class AtualizarClienteHandler : IRequestHandler<AtualizarClienteCommand>
{
    private readonly AppDbContext _context;

    public AtualizarClienteHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(AtualizarClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = await _context.Clientes
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
            ?? throw new InvalidOperationException("Cliente não encontrado.");

        cliente.AtualizarDados(request.NomeCompleto, request.Telefone);

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

        await _context.SaveChangesAsync(cancellationToken);
    }
}