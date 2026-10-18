using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.OrdensServico.Commands;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.OrdensServico;

public class AtualizarNomeItemServicoOSHandler : IRequestHandler<AtualizarNomeItemServicoOSCommand>
{
    private readonly AppDbContext _context;

    public AtualizarNomeItemServicoOSHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(AtualizarNomeItemServicoOSCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.ItensServicoOS
            .FirstOrDefaultAsync(i => i.Id == request.ItemId, cancellationToken)
            ?? throw new InvalidOperationException("Item de serviço não encontrado.");

        item.AtualizarNomePersonalizado(request.NovoNome);

        await _context.SaveChangesAsync(cancellationToken);
    }
}