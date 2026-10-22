using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Common.DTOs;
using SgaAutoEletrica.Application.Features.Veiculos.DTOs;
using SgaAutoEletrica.Application.Features.Veiculos.Queries;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Queries.Veiculos;

public class BuscarVeiculosHandler : IRequestHandler<BuscarVeiculosQuery, ListaPaginadaDTO<VeiculoDTO>>
{
    private readonly AppDbContext _context;

    public BuscarVeiculosHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ListaPaginadaDTO<VeiculoDTO>> Handle(BuscarVeiculosQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Veiculos
            .Include(v => v.Cliente)
            .Include(v => v.MarcaVeiculo)
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

        if (!string.IsNullOrWhiteSpace(request.Placa))
        {
            var termo = request.Placa.Trim().ToLower();
            query = query.Where(v => v.Placa.Valor.ToLower().Contains(termo));
        }

        if (!string.IsNullOrWhiteSpace(request.Modelo))
        {
            var termo = request.Modelo.Trim().ToLower();
            query = query.Where(v => v.Modelo.ToLower().Contains(termo));
        }

        if (request.MarcaId.HasValue)
        {
            if (request.MarcaId.Value == Guid.Parse("00000000-0000-0000-0000-000000000001"))
            {
                query = query.Where(v => v.MarcaVeiculoId == null);
            }
            else
            {
                var marcaNome = await _context.MarcasVeiculo
                    .Where(m => m.Id == request.MarcaId.Value)
                    .Select(m => m.Nome)
                    .FirstOrDefaultAsync(cancellationToken);

                query = query.Where(v => v.MarcaVeiculoId == request.MarcaId.Value
                    || (v.MarcaVeiculoId == null && v.Marca == marcaNome));
            }
        }

        if (!string.IsNullOrWhiteSpace(request.NomeCliente))
        {
            var termo = request.NomeCliente.Trim().ToLower();
            query = query.Where(v => v.Cliente.NomeCompleto.ToLower().Contains(termo));
        }

        if (request.Ano.HasValue)
        {
            query = query.Where(v => v.Ano == request.Ano.Value);
        }

        if (request.Ativo.HasValue)
            query = query.Where(v => v.Ativo == request.Ativo.Value);

        var totalItens = await query.CountAsync(cancellationToken);

        var itens = await query
            .OrderBy(v => v.Cliente.NomeCompleto)
            .ThenBy(v => v.Modelo)
            .Skip((request.Pagina - 1) * request.TamanhoPagina)
            .Take(request.TamanhoPagina)
            .Select(v => new VeiculoDTO
            {
                Id = v.Id,
                Placa = v.Placa.Valor,
                Modelo = v.Modelo,
                Marca = v.Marca,
                MarcaVeiculoId = v.MarcaVeiculoId,
                MarcaVeiculoNome = v.MarcaVeiculo != null ? v.MarcaVeiculo.Nome : null,
                Ano = v.Ano,
                Versao = v.Versao,
                Motor = v.Motor,
                TipoMotor = v.TipoMotor,
                Cor = v.Cor,
                Observacao = v.Observacao,
                ClienteId = v.ClienteId,
                NomeCliente = v.Cliente.NomeCompleto,
                CpfCliente = v.Cliente.CpfCnpj != null ? v.Cliente.CpfCnpj.Formatado() : null,
                TelefoneCliente = v.Cliente.Telefone != null ? v.Cliente.Telefone.Formatado() : null,
                Ativo = v.Ativo
            })
            .ToListAsync(cancellationToken);

        return new ListaPaginadaDTO<VeiculoDTO>
        {
            Itens = itens,
            PaginaAtual = request.Pagina,
            TamanhoPagina = request.TamanhoPagina,
            TotalItens = totalItens
        };
    }
}