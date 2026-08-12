using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Veiculos.DTOs;
using SgaAutoEletrica.Application.Features.Veiculos.Queries;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Queries.Veiculos;

public class BuscarVeiculosHandler : IRequestHandler<BuscarVeiculosQuery, List<VeiculoDTO>>
{
    private readonly AppDbContext _context;

    public BuscarVeiculosHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<VeiculoDTO>> Handle(BuscarVeiculosQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Veiculos
            .Include(v => v.Cliente)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Placa))
            query = query.Where(v => v.Placa.Valor.Contains(request.Placa));

        if (!string.IsNullOrWhiteSpace(request.Marca))
            query = query.Where(v => v.Marca.Contains(request.Marca));

        if (!string.IsNullOrWhiteSpace(request.Modelo))
            query = query.Where(v => v.Modelo.Contains(request.Modelo));

        if (request.Ano.HasValue)
            query = query.Where(v => v.Ano == request.Ano.Value);

        if (!string.IsNullOrWhiteSpace(request.NomeCliente))
            query = query.Where(v => v.Cliente.NomeCompleto.Contains(request.NomeCliente));

        return await query
            .OrderBy(v => v.Cliente.NomeCompleto)
            .ThenBy(v => v.Modelo)
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