using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Veiculos.Commands;
using SgaAutoEletrica.Domain.Enums;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.Veiculos;

public class AtualizarVeiculoHandler : IRequestHandler<AtualizarVeiculoCommand>
{
    private readonly AppDbContext _context;

    public AtualizarVeiculoHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(AtualizarVeiculoCommand request, CancellationToken cancellationToken)
    {
        var veiculo = await _context.Veiculos
            .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken)
            ?? throw new InvalidOperationException("Veículo não encontrado.");

        TipoMotor? tipoMotor = null;
        if (!string.IsNullOrWhiteSpace(request.TipoMotor) &&
            Enum.TryParse<TipoMotor>(request.TipoMotor, out var parsed))
        {
            tipoMotor = parsed;
        }

        veiculo.AtualizarDados(
            request.Placa,
            request.Modelo,
            request.Marca,
            request.Ano,
            request.Versao,
            request.Motor,
            tipoMotor,
            request.Cor,
            request.Observacao);

        await _context.SaveChangesAsync(cancellationToken);
    }
}