using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Clientes.Commands;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.Clientes;

public class ExcluirClienteHandler : IRequestHandler<ExcluirClienteCommand>
{
    private readonly AppDbContext _context;

    public ExcluirClienteHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ExcluirClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = await _context.Clientes
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
            ?? throw new InvalidOperationException("Cliente não encontrado.");

        _context.Clientes.Remove(cliente);
        await _context.SaveChangesAsync(cancellationToken);
    }
}