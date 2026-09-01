using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Configuracoes.Commands;
using SgaAutoEletrica.Domain.Entities;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.Configuracoes;

public class SalvarConfiguracaoImpressoraHandler : IRequestHandler<SalvarConfiguracaoImpressoraCommand>
{
    private readonly AppDbContext _context;

    public SalvarConfiguracaoImpressoraHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(SalvarConfiguracaoImpressoraCommand request, CancellationToken cancellationToken)
    {
        var existente = await _context.ConfiguracoesImpressora
            .FirstOrDefaultAsync(c => c.Tipo == request.Tipo, cancellationToken);

        if (existente != null)
        {
            existente.Atualizar(
                request.NomeImpressora,
                request.TamanhoPapel,
                request.Copias,
                request.MargemSuperior,
                request.MargemInferior,
                request.MargemEsquerda,
                request.MargemDireita);
        }
        else
        {
            var nova = new ConfiguracaoImpressora(
                request.Tipo,
                request.NomeImpressora,
                request.TamanhoPapel,
                request.Copias,
                request.MargemSuperior,
                request.MargemInferior,
                request.MargemEsquerda,
                request.MargemDireita);

            await _context.ConfiguracoesImpressora.AddAsync(nova, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}