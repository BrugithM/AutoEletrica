using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.OrdensServico.Commands;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.OrdensServico;

public class AtualizarClienteVeiculoOSHandler : IRequestHandler<AtualizarClienteVeiculoOSCommand>
{
    private readonly AppDbContext _context;

    public AtualizarClienteVeiculoOSHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(AtualizarClienteVeiculoOSCommand request, CancellationToken cancellationToken)
    {
        var os = await _context.OrdensServico
            .FirstOrDefaultAsync(o => o.Id == request.OrdemServicoId, cancellationToken)
            ?? throw new InvalidOperationException("OS não encontrada.");

        os.AlterarClienteVeiculo(request.ClienteId, request.VeiculoId);

        await _context.SaveChangesAsync(cancellationToken);
    }
}