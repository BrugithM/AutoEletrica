using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.OrdensServico.DTOs;
using SgaAutoEletrica.Application.Features.OrdensServico.Queries;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Queries.OrdensServico;

public class ObterOSPorIdHandler : IRequestHandler<ObterOSPorIdQuery, OrdemServicoDetalheDTO?>
{
    private readonly AppDbContext _context;

    public ObterOSPorIdHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<OrdemServicoDetalheDTO?> Handle(ObterOSPorIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.OrdensServico
            .AsNoTracking()
            .Include(os => os.Cliente)
            .Include(os => os.Veiculo)
            .Include(os => os.ItensPeca).ThenInclude(i => i.Peca)
            .Include(os => os.ItensServico).ThenInclude(i => i.Servico)
            .Where(os => os.Id == request.Id)
            .Select(os => new OrdemServicoDetalheDTO
            {
                Id = os.Id,
                Numero = os.Numero,
                Status = os.Status,
                Observacao = os.Observacao,
                DataAbertura = os.DataAbertura,
                DataFinalizacao = os.DataFinalizacao,
                ClienteId = os.ClienteId,
                NomeCliente = os.Cliente.NomeCompleto,
                TelefoneCliente = os.Cliente.Telefone.Valor,
                VeiculoId = os.VeiculoId,
                PlacaVeiculo = os.Veiculo.Placa.Valor,
                ModeloVeiculo = os.Veiculo.Modelo,
                MarcaVeiculo = os.Veiculo.Marca,
                AnoVeiculo = os.Veiculo.Ano,
                ValorTotalPecas = os.ValorTotalPecas,
                ValorTotalServicos = os.ValorTotalServicos,
                ValorTotal = os.ValorTotal,
                ItensPeca = os.ItensPeca.Select(i => new ItemPecaOSDTO
                {
                    Id = i.Id,
                    PecaId = i.PecaId,
                    NomePeca = i.Peca.Nome,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario,
                    ValorTotal = i.ValorTotal
                }).ToList(),
                ItensServico = os.ItensServico.Select(i => new ItemServicoOSDTO
                {
                    Id = i.Id,
                    ServicoId = i.ServicoId,
                    NomeServico = i.Servico.Nome,
                    PrecoUnitario = i.PrecoUnitario
                }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}