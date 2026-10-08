using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.OrdensServico.Commands;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.OrdensServico;

public class AtualizarQuilometragemOSHandler : IRequestHandler<AtualizarQuilometragemOSCommand>
{
    private readonly AppDbContext _context;

    public AtualizarQuilometragemOSHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(AtualizarQuilometragemOSCommand request, CancellationToken cancellationToken)
    {
        var os = await _context.OrdensServico
            .FirstOrDefaultAsync(o => o.Id == request.OrdemServicoId, cancellationToken)
            ?? throw new InvalidOperationException("OS não encontrada.");

        os.AtualizarQuilometragem(request.Quilometragem);
        await _context.SaveChangesAsync(cancellationToken);
    }
}