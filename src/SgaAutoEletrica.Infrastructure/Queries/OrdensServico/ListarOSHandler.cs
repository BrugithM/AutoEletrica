using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.OrdensServico.DTOs;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Application.Features.OrdensServico.Queries;

public class ListarOSHandler : IRequestHandler<ListarOSQuery, List<OrdemServicoResumoDTO>>
{
    private readonly AppDbContext _context;

    public ListarOSHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<OrdemServicoResumoDTO>> Handle(ListarOSQuery request, CancellationToken cancellationToken)
    {
        var query = _context.OrdensServico
            .Include(os => os.Cliente)
            .Include(os => os.Veiculo)
            .AsQueryable();

        if (request.Status.HasValue)
            query = query.Where(os => os.Status == request.Status.Value);

        if (request.DataInicio.HasValue)
            query = query.Where(os => os.DataAbertura >= request.DataInicio.Value);

        if (request.DataFim.HasValue)
            query = query.Where(os => os.DataAbertura <= request.DataFim.Value);

        if (!string.IsNullOrWhiteSpace(request.TermoBusca))
            query = query.Where(os => 
                os.Cliente.NomeCompleto.Contains(request.TermoBusca) ||
                os.Veiculo.Placa.Valor.Contains(request.TermoBusca) ||
                os.Numero.ToString().Contains(request.TermoBusca));

        return await query
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