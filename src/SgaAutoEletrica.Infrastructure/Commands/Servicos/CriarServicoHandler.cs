using MediatR;
using SgaAutoEletrica.Application.Features.Servicos.Commands;
using SgaAutoEletrica.Domain.Entities;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.Servicos;

public class CriarServicoHandler : IRequestHandler<CriarServicoCommand, Guid>
{
    private readonly AppDbContext _context;

    public CriarServicoHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CriarServicoCommand request, CancellationToken cancellationToken)
    {
        var servico = new Servico(request.Nome, request.PrecoPadrao, request.Descricao);

        await _context.Servicos.AddAsync(servico, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return servico.Id;
    }
}