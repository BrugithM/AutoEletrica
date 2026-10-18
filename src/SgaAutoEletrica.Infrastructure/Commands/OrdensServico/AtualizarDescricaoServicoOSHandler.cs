using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.OrdensServico.Commands;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.OrdensServico;

public class AtualizarDescricaoServicosOSHandler : IRequestHandler<AtualizarDescricaoServicosOSCommand>
{
    private readonly AppDbContext _context;

    public AtualizarDescricaoServicosOSHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(AtualizarDescricaoServicosOSCommand request, CancellationToken cancellationToken)
    {
        var os = await _context.OrdensServico
            .FirstOrDefaultAsync(o => o.Id == request.OrdemServicoId, cancellationToken)
            ?? throw new InvalidOperationException("OS não encontrada.");

        os.AtualizarDescricaoServicos(request.DescricaoServicos);

        await _context.SaveChangesAsync(cancellationToken);
    }
}