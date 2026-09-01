using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.OrdensServico.Commands;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.OrdensServico;

public class AtualizarObservacaoOSHandler : IRequestHandler<AtualizarObservacaoOSCommand>
{
    private readonly AppDbContext _context;

    public AtualizarObservacaoOSHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(AtualizarObservacaoOSCommand request, CancellationToken cancellationToken)
    {
        var os = await _context.OrdensServico
            .FirstOrDefaultAsync(o => o.Id == request.OrdemServicoId, cancellationToken)
            ?? throw new InvalidOperationException("OS não encontrada.");

        os.AdicionarObservacao(request.Observacao);
        await _context.SaveChangesAsync(cancellationToken);
    }
}