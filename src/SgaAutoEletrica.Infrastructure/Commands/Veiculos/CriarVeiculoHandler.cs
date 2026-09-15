using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Veiculos.Commands;
using SgaAutoEletrica.Domain.Entities;
using SgaAutoEletrica.Domain.Enums;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.Veiculos;

public class CriarVeiculoHandler : IRequestHandler<CriarVeiculoCommand, Guid>
{
    private readonly AppDbContext _context;

    public CriarVeiculoHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CriarVeiculoCommand request, CancellationToken cancellationToken)
    {
        Veiculo veiculo;

        if (!string.IsNullOrWhiteSpace(request.Modelo) &&
            !string.IsNullOrWhiteSpace(request.Marca) &&
            request.Ano.HasValue)
        {
            veiculo = new Veiculo(request.Placa, request.ClienteId, request.Modelo, request.Marca, request.Ano.Value);

            if (!string.IsNullOrWhiteSpace(request.Versao) ||
                !string.IsNullOrWhiteSpace(request.Motor) ||
                !string.IsNullOrWhiteSpace(request.Cor) ||
                !string.IsNullOrWhiteSpace(request.Observacao))
            {
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
                    request.Ano.Value,
                    request.Versao,
                    request.Motor,
                    tipoMotor,
                    request.Cor,
                    request.Observacao);
            }
        }
        else
        {
            veiculo = new Veiculo(request.Placa, request.ClienteId);
        }

        await _context.Veiculos.AddAsync(veiculo, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return veiculo.Id;
    }
}