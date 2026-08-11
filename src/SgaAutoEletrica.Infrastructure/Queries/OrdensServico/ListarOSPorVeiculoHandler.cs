using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.OrdensServico.DTOs;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Application.Features.OrdensServico.Queries;

public class ListarOSPorVeiculoHandler : IRequestHandler<ListarOSPorVeiculoQuery, List<OrdemServicoResumoDTO>>
{
    private readonly AppDbContext _context;

    public ListarOSPorVeiculoHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<OrdemServicoResumoDTO>> Handle(ListarOSPorVeiculoQuery request, CancellationToken cancellationToken)
    {
        return await _context.OrdensServico
            .Include(os => os.Cliente)
            .Include(os => os.Veiculo)
            .Where(os => os.VeiculoId == request.VeiculoId)
            .OrderByDescending(os => os.DataAbertura)
            .Select(os => new OrdemServicoResumoDTO
            {
                Id = os.Id,
                Numero = os.Numero,
                NomeCliente = os.Cliente.NomeCompleto,
                PlacaVeiculo = os.Veiculo.Placa.Valor,
                ModeloVeiculo = os.Veiculo.Modelo,
                Status = os.Status,
                ValorTotal = os.ValorTotal,
                DataAbertura = os.DataAbertura,
                DataFinalizacao = os.DataFinalizacao
            })
            .ToListAsync(cancellationToken);
    }
}