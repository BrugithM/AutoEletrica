using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.OrdensServico.Commands;
using SgaAutoEletrica.Domain.Entities;
using SgaAutoEletrica.Domain.Enums;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.OrdensServico;

public class GerarNotaFiscalSaidaHandler : IRequestHandler<GerarNotaFiscalSaidaCommand, Guid>
{
    private readonly AppDbContext _context;

    public GerarNotaFiscalSaidaHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(GerarNotaFiscalSaidaCommand request, CancellationToken cancellationToken)
    {
        var os = await _context.OrdensServico
            .Include(o => o.ItensPeca).ThenInclude(i => i.Peca)
            .Include(o => o.ItensServico).ThenInclude(i => i.Servico)
            .FirstOrDefaultAsync(o => o.Id == request.OrdemServicoId, cancellationToken)
            ?? throw new InvalidOperationException("OS não encontrada.");

        if (os.Status != StatusOS.Finalizada)
            throw new InvalidOperationException("Só é possível gerar NF para OS finalizada.");

        var nfExistente = await _context.NotasFiscaisSaida
            .AnyAsync(nf => nf.OrdemServicoId == os.Id, cancellationToken);

        if (nfExistente)
            throw new InvalidOperationException("Esta OS já possui Nota Fiscal.");

        var nf = new NotaFiscalSaida(request.NumeroNota, os);

        foreach (var item in os.ItensPeca)
        {
            nf.AdicionarItem(item.Peca.Nome, item.Quantidade, item.PrecoUnitario);
        }

        foreach (var item in os.ItensServico)
        {
            nf.AdicionarItem(item.Servico.Nome, 1, item.PrecoUnitario);
        }

        await _context.NotasFiscaisSaida.AddAsync(nf, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return nf.Id;
    }
}