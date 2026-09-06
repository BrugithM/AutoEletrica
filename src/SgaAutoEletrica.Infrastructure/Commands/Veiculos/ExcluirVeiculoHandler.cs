using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Veiculos.Commands;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.Veiculos;

public class ExcluirVeiculoHandler : IRequestHandler<ExcluirVeiculoCommand>
{
    private readonly AppDbContext _context;

    public ExcluirVeiculoHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ExcluirVeiculoCommand request, CancellationToken cancellationToken)
    {
        var veiculo = await _context.Veiculos
            .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken)
            ?? throw new InvalidOperationException("Veículo não encontrado.");

        _context.Veiculos.Remove(veiculo);
        await _context.SaveChangesAsync(cancellationToken);
    }
}