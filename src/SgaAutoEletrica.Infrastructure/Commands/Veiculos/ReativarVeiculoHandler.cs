using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Veiculos.Commands;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.Veiculos;

public class ReativarVeiculoHandler : IRequestHandler<ReativarVeiculoCommand>
{
    private readonly AppDbContext _context;

    public ReativarVeiculoHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ReativarVeiculoCommand request, CancellationToken cancellationToken)
    {
        var veiculo = await _context.Veiculos
            .Include(v => v.Cliente)
            .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken)
            ?? throw new InvalidOperationException("Veículo não encontrado.");

        if (!veiculo.Cliente.Ativo)
            throw new InvalidOperationException(
                "Não é possível reativar um veículo cujo cliente está inativo. " +
                "Reative o cliente ou altere o proprietário do veículo.");

        veiculo.Ativar();
        await _context.SaveChangesAsync(cancellationToken);
    }
}