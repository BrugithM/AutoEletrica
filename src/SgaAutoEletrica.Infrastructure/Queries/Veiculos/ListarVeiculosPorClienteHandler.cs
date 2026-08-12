using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Veiculos.DTOs;
using SgaAutoEletrica.Application.Features.Veiculos.Queries;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Queries.Veiculos;

public class ListarVeiculosPorClienteHandler : IRequestHandler<ListarVeiculosPorClienteQuery, List<VeiculoDTO>>
{
    private readonly AppDbContext _context;

    public ListarVeiculosPorClienteHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<VeiculoDTO>> Handle(ListarVeiculosPorClienteQuery request, CancellationToken cancellationToken)
    {
        return await _context.Veiculos
            .Where(v => v.ClienteId == request.ClienteId)
            .OrderBy(v => v.Modelo)
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
            .ToListAsync(cancellationToken);
    }
}