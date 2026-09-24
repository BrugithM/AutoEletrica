using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.OrdensServico.Commands;
using SgaAutoEletrica.Domain.Entities;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.OrdensServico;

public class CriarOrdemServicoHandler : IRequestHandler<CriarOrdemServicoCommand, Guid>
{
    private readonly AppDbContext _context;

    public CriarOrdemServicoHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CriarOrdemServicoCommand request, CancellationToken cancellationToken)
    {
        var ultimo = await _context.OrdensServico
            .OrderByDescending(os => os.Numero)
            .Select(os => (int?)os.Numero)
            .FirstOrDefaultAsync(cancellationToken) ?? 0;

        var proximo = ultimo + 1;

        if (proximo <= 0)
            proximo = 1;

        while (await _context.OrdensServico.AnyAsync(os => os.Numero == proximo, cancellationToken))
            proximo++;

        var os = new OrdemServico(proximo, request.ClienteId, request.VeiculoId, request.Observacao);

        foreach (var item in request.Pecas)
        {
            var peca = await _context.Pecas.FindAsync([item.PecaId], cancellationToken)
                ?? throw new InvalidOperationException("Peça não encontrada.");

            peca.DarBaixaEstoque(item.Quantidade);
            os.AdicionarPeca(item.PecaId, item.Quantidade, peca.ValorVenda);
        }

        foreach (var item in request.Servicos)
        {
            var servico = await _context.Servicos.FindAsync([item.ServicoId], cancellationToken)
                ?? throw new InvalidOperationException("Serviço não encontrado.");

            os.AdicionarServico(item.ServicoId, servico.PrecoPadrao);
        }

        if (request.Desconto > 0)
            os.AplicarDesconto(request.Desconto);

        if (request.AprovarIniciar)
            os.IniciarServico();

        await _context.OrdensServico.AddAsync(os, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return os.Id;
    }
}