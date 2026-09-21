using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Servicos.Commands;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.Servicos;

public class DesativarServicoHandler : IRequestHandler<DesativarServicoCommand>
{
    private readonly AppDbContext _context;

    public DesativarServicoHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DesativarServicoCommand request, CancellationToken cancellationToken)
    {
        var servico = await _context.Servicos
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken)
            ?? throw new InvalidOperationException("Serviço não encontrado.");

        servico.Desativar();
        await _context.SaveChangesAsync(cancellationToken);
    }
}