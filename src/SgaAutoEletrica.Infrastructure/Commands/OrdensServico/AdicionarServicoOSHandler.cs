using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.OrdensServico.Commands;
using SgaAutoEletrica.Domain.Entities;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.OrdensServico;

public class AdicionarServicoOSHandler : IRequestHandler<AdicionarServicoOSCommand>
{
    private readonly AppDbContext _context;

    public AdicionarServicoOSHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(AdicionarServicoOSCommand request, CancellationToken cancellationToken)
    {
        var servico = await _context.Servicos
            .FirstOrDefaultAsync(s => s.Id == request.ServicoId, cancellationToken)
            ?? throw new InvalidOperationException("Serviço não encontrado.");

        var os = await _context.OrdensServico
            .FirstOrDefaultAsync(o => o.Id == request.OrdemServicoId, cancellationToken)
            ?? throw new InvalidOperationException("OS não encontrada.");

        var novoItem = new ItemServicoOS(request.ServicoId, servico.PrecoPadrao);
        novoItem.DefinirOrdemServico(request.OrdemServicoId);
        _context.ItensServicoOS.Add(novoItem);

        var totalPecas = await _context.ItensPecaOS
            .Where(i => i.OrdemServicoId == request.OrdemServicoId)
            .SumAsync(i => i.ValorTotal, cancellationToken);

        var totalServicos = await _context.ItensServicoOS
            .Where(i => i.OrdemServicoId == request.OrdemServicoId)
            .SumAsync(i => i.PrecoUnitario, cancellationToken);

        os.AtualizarTotais(totalPecas, totalServicos);

        await _context.SaveChangesAsync(cancellationToken);
    }
}