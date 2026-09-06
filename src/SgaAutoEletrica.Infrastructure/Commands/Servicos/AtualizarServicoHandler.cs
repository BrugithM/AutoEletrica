using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Servicos.Commands;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.Servicos;

public class AtualizarServicoHandler : IRequestHandler<AtualizarServicoCommand>
{
    private readonly AppDbContext _context;

    public AtualizarServicoHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(AtualizarServicoCommand request, CancellationToken cancellationToken)
    {
        var servico = await _context.Servicos
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken)
            ?? throw new InvalidOperationException("Serviço não encontrado.");

        servico.AtualizarPreco(request.PrecoPadrao);
        servico.AtualizarDados(request.Nome, request.Descricao);

        await _context.SaveChangesAsync(cancellationToken);
    }
}