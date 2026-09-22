using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Clientes.Commands;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.Clientes;

public class ReativarClienteHandler : IRequestHandler<ReativarClienteCommand>
{
    private readonly AppDbContext _context;

    public ReativarClienteHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ReativarClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = await _context.Clientes
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
            ?? throw new InvalidOperationException("Cliente não encontrado.");

        cliente.Ativar();
        await _context.SaveChangesAsync(cancellationToken);
    }
}