using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.OrdensServico.Commands;
using SgaAutoEletrica.Domain.Enums;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.OrdensServico;

public class AlterarStatusOSHandler : IRequestHandler<AlterarStatusOSCommand>
{
    private readonly AppDbContext _context;

    public AlterarStatusOSHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(AlterarStatusOSCommand request, CancellationToken cancellationToken)
    {
        var os = await _context.OrdensServico
            .Include(o => o.ItensPeca)
            .Include(o => o.ItensServico)
            .FirstOrDefaultAsync(o => o.Id == request.OrdemServicoId, cancellationToken)
            ?? throw new InvalidOperationException("OS não encontrada.");

        switch (request.NovoStatus)
        {
            case StatusOS.EmAndamento:
                os.IniciarServico();
                break;

            case StatusOS.AguardandoPecas:
                os.AguardarPecas();
                break;

            case StatusOS.Finalizada:
                os.Finalizar();
                break;

            case StatusOS.Cancelada:
                // Devolve peças ao estoque
                foreach (var item in os.ItensPeca)
                {
                    var peca = await _context.Pecas.FindAsync([item.PecaId], cancellationToken);
                    if (peca != null)
                    {
                        peca.DarEntradaEstoque(item.Quantidade);
                    }
                }
                os.Cancelar(request.MotivoCancelamento ?? "Cancelado pelo usuário");
                break;

            default:
                throw new InvalidOperationException($"Status {request.NovoStatus} não é válido para esta operação.");
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}