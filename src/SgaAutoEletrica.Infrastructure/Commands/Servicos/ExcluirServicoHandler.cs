using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Servicos.Commands;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.Servicos;

public class ExcluirServicoHandler : IRequestHandler<ExcluirServicoCommand>
{
    private readonly AppDbContext _context;

    public ExcluirServicoHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ExcluirServicoCommand request, CancellationToken cancellationToken)
    {
        var servico = await _context.Servicos
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken)
            ?? throw new InvalidOperationException("Serviço não encontrado.");

        _context.Servicos.Remove(servico);
        await _context.SaveChangesAsync(cancellationToken);
    }
}