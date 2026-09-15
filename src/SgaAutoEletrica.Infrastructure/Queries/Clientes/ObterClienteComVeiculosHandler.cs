using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Clientes.DTOs;
using SgaAutoEletrica.Application.Features.Clientes.Queries;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Queries.Clientes;

public class ObterClienteComVeiculosHandler : IRequestHandler<ObterClienteComVeiculosQuery, ClienteComVeiculosDTO?>
{
    private readonly AppDbContext _context;

    public ObterClienteComVeiculosHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ClienteComVeiculosDTO?> Handle(ObterClienteComVeiculosQuery request, CancellationToken cancellationToken)
    {
        return await _context.Clientes
            .AsNoTracking()
            .Include(c => c.Veiculos)
            .Where(c => c.Id == request.ClienteId)
            .Select(c => new ClienteComVeiculosDTO
            {
                Id = c.Id,
                NomeCompleto = c.NomeCompleto,
                Cpf = c.Cpf.Formatado(),
                Telefone = c.Telefone.Formatado(),
                EnderecoCompleto = c.Endereco != null ? c.Endereco.Completo() : null,
                Veiculos = c.Veiculos.Select(v => new VeiculoResumoDTO
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
                    Observacao = v.Observacao
                }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}