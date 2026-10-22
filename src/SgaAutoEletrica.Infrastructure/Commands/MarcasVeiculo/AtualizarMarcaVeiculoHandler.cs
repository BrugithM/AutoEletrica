using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.MarcasVeiculo.Commands;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.MarcasVeiculo;

public class AtualizarMarcaVeiculoHandler : IRequestHandler<AtualizarMarcaVeiculoCommand>
{
    private readonly AppDbContext _context;

    public AtualizarMarcaVeiculoHandler(AppDbContext context) => _context = context;

    public async Task Handle(AtualizarMarcaVeiculoCommand request, CancellationToken cancellationToken)
    {
        var marca = await _context.MarcasVeiculo
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken)
            ?? throw new InvalidOperationException("Marca não encontrada.");

        marca.AtualizarNome(request.Nome);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

public class AlternarAtivoMarcaVeiculoHandler : IRequestHandler<AlternarAtivoMarcaVeiculoCommand>
{
    private readonly AppDbContext _context;

    public AlternarAtivoMarcaVeiculoHandler(AppDbContext context) => _context = context;

    public async Task Handle(AlternarAtivoMarcaVeiculoCommand request, CancellationToken cancellationToken)
    {
        var marca = await _context.MarcasVeiculo
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken)
            ?? throw new InvalidOperationException("Marca não encontrada.");

        if (request.Ativo)
            marca.Ativar();
        else
            marca.Desativar();

        await _context.SaveChangesAsync(cancellationToken);
    }
}