using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Buscas.DTOs;
using SgaAutoEletrica.Application.Features.Buscas.Queries;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Queries.Buscas;

public class ObterNotaFiscalSaidaPorIdHandler : IRequestHandler<ObterNotaFiscalSaidaPorIdQuery, NotaFiscalSaidaDetalheDTO?>
{
    private readonly AppDbContext _context;

    public ObterNotaFiscalSaidaPorIdHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<NotaFiscalSaidaDetalheDTO?> Handle(ObterNotaFiscalSaidaPorIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.NotasFiscaisSaida
            .Include(nf => nf.Cliente)
            .Include(nf => nf.Veiculo)
            .Include(nf => nf.Itens)
            .AsNoTracking()
            .Where(nf => nf.Id == request.Id)
            .Select(nf => new NotaFiscalSaidaDetalheDTO
            {
                Id = nf.Id,
                Numero = nf.Numero,
                DataEmissao = nf.DataEmissao,
                NomeCliente = nf.Cliente.NomeCompleto,
                TelefoneCliente = nf.Cliente.Telefone.Formatado(),
                PlacaVeiculo = nf.Veiculo.Placa.Valor,
                ModeloVeiculo = nf.Veiculo.Modelo,
                MarcaVeiculo = nf.Veiculo.Marca,
                ValorTotal = nf.ValorTotal,
                Observacao = nf.Observacao,
                Itens = nf.Itens.Select(i => new ItemNotaFiscalSaidaDetalheDTO
                {
                    Descricao = i.Descricao,
                    Quantidade = i.Quantidade,
                    ValorUnitario = i.ValorUnitario,
                    ValorTotal = i.ValorTotal
                }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}