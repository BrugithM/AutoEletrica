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
        var os = await _context.OrdensServico
            .AsNoTracking()
            .Include(os => os.Cliente)
            .Include(os => os.Veiculo)
            .Include(os => os.ItensPeca).ThenInclude(i => i.Peca)
            .Include(os => os.ItensServico).ThenInclude(i => i.Servico)
            .FirstOrDefaultAsync(os => os.Id == request.Id, cancellationToken);

        if (os == null) return null;

        return new OrdemServicoDetalheDTO
        {
            Id = os.Id,
            Numero = os.Numero,
            Status = os.Status,
            Observacao = os.Observacao,
            DescricaoServicos = os.DescricaoServicos,
            DataAbertura = os.DataAbertura,
            DataFinalizacao = os.DataFinalizacao,
            ClienteId = os.ClienteId,
            NomeCliente = os.Cliente.NomeCompleto,
            CpfCliente = os.Cliente.CpfCnpj != null ? os.Cliente.CpfCnpj.Formatado() : null,
            TelefoneCliente = os.Cliente.Telefone != null ? os.Cliente.Telefone.Formatado() : null,
            VeiculoId = os.VeiculoId,
            PlacaVeiculo = os.Veiculo.Placa.Valor,
            ModeloVeiculo = os.Veiculo.Modelo,
            MarcaVeiculo = os.Veiculo.Marca,
            AnoVeiculo = os.Veiculo.Ano,
            Quilometragem = os.Quilometragem,
            ValorTotalPecas = os.ValorTotalPecas,
            ValorTotalServicos = os.ValorTotalServicos,
            DescontoPercentual = os.DescontoPercentual,
            ValorTotal = os.ValorTotal,
            ItensPeca = os.ItensPeca
                .OrderBy(i => i.Peca.Nome, StringComparer.OrdinalIgnoreCase)
                .Select(i => new ItemPecaOSDTO
                {
                    Id = i.Id,
                    PecaId = i.PecaId,
                    NomePeca = i.Peca.Nome,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario,
                    ValorTotal = i.ValorTotal
                }).ToList(),
            ItensServico = os.ItensServico
                .Select(i => new ItemServicoOSDTO
                {
                    Id = i.Id,
                    ServicoId = i.ServicoId,
                    NomeServico = i.NomePersonalizado ?? i.Servico.Nome,
                    NomePersonalizado = i.NomePersonalizado,
                    PrecoUnitario = i.PrecoUnitario
                })
                .OrderBy(i => i.NomeServico, StringComparer.OrdinalIgnoreCase)
                .ToList()
        };
    }
}