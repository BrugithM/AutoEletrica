using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Common.DTOs;
using SgaAutoEletrica.Application.Features.OrdensServico.DTOs;
using SgaAutoEletrica.Application.Features.OrdensServico.Queries;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Queries.OrdensServico;

public class ListarOSHandler : IRequestHandler<ListarOSQuery, ListaPaginadaDTO<OrdemServicoResumoDTO>>
{
    private readonly AppDbContext _context;

    public ListarOSHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ListaPaginadaDTO<OrdemServicoResumoDTO>> Handle(ListarOSQuery request, CancellationToken cancellationToken)
    {
        var query = _context.OrdensServico
            .Include(os => os.Cliente)
            .Include(os => os.Veiculo)
            .AsNoTracking()
            .AsQueryable();

        if (request.Status.HasValue)
            query = query.Where(os => os.Status == request.Status.Value);

        if (!string.IsNullOrWhiteSpace(request.TermoBusca))
        {
            var termo = request.TermoBusca.Trim().ToLower();
            query = query.Where(os =>
                os.Cliente.NomeCompleto.ToLower().Contains(termo) ||
                os.Veiculo.Placa.Valor.ToLower().Contains(termo) ||
                os.Numero.ToString().Contains(termo));
        }

        var totalItens = await query.CountAsync(cancellationToken);

        var itens = await query
            .OrderByDescending(os => os.DataAbertura)
            .Skip((request.Pagina - 1) * request.TamanhoPagina)
            .Take(request.TamanhoPagina)
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

        return new ListaPaginadaDTO<OrdemServicoResumoDTO>
        {
            Itens = itens,
            PaginaAtual = request.Pagina,
            TamanhoPagina = request.TamanhoPagina,
            TotalItens = totalItens
        };
    }
}