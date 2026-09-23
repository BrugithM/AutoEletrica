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
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.TermoBusca))
        {
            var termo = request.TermoBusca.Trim().ToLower();
            query = query.Where(v =>
                v.Placa.Valor.ToLower().Contains(termo) ||
                v.Modelo.ToLower().Contains(termo) ||
                v.Marca.ToLower().Contains(termo) ||
                v.Cliente.NomeCompleto.ToLower().Contains(termo));
        }
        if(request.Ativo.HasValue)
            query = query.Where(v =>v.Ativo == request.Ativo.Value);

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
                NomeCliente = v.Cliente.NomeCompleto,
                CpfCliente = v.Cliente.Cpf.Formatado(),
                TelefoneCliente = v.Cliente.Telefone.Formatado(),
                Ativo = v.Ativo
            })
            .ToListAsync(cancellationToken);
    }
}