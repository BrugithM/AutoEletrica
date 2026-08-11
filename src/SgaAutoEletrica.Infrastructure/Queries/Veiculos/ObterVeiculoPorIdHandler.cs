using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Veiculos.DTOs;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Application.Features.Veiculos.Queries;

public class ObterVeiculoPorIdHandler : IRequestHandler<ObterVeiculoPorIdQuery, VeiculoDTO?>
{
    private readonly AppDbContext _context;

    public ObterVeiculoPorIdHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<VeiculoDTO?> Handle(ObterVeiculoPorIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Veiculos
            .Where(v => v.Id == request.Id)
            .Select(v => new VeiculoDTO
            {
                Id = v.Id,
                Placa = v.Placa.Valor,
                Modelo = v.Modelo,
                Marca = v.Marca,
                Ano = v.Ano,
                Versao = v.Versao,
                Motor = v.Motor,
                TipoMotor = v.TipoMotor != null ? v.TipoMotor.ToString() : null,
                Cor = v.Cor,
                Observacao = v.Observacao,
                ClienteId = v.ClienteId,
                NomeCliente = v.Cliente.NomeCompleto
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}