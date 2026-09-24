using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.OrdensServico.Commands;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.OrdensServico;

public class AplicarDescontoOSHandler : IRequestHandler<AplicarDescontoOSCommand>
{
    private readonly AppDbContext _context;

    public AplicarDescontoOSHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(AplicarDescontoOSCommand request, CancellationToken cancellationToken)
    {
        var os = await _context.OrdensServico
            .Include(o => o.ItensPeca)
            .Include(o => o.ItensServico)
            .FirstOrDefaultAsync(o => o.Id == request.OrdemServicoId, cancellationToken)
            ?? throw new InvalidOperationException("OS não encontrada.");

        os.AplicarDesconto(request.Desconto);
        await _context.SaveChangesAsync(cancellationToken);
    }
}