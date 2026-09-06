using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Configuracoes.Commands;
using SgaAutoEletrica.Domain.Entities;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.Configuracoes;

public class SalvarConfiguracaoEmpresaHandler : IRequestHandler<SalvarConfiguracaoEmpresaCommand>
{
    private readonly AppDbContext _context;

    public SalvarConfiguracaoEmpresaHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(SalvarConfiguracaoEmpresaCommand request, CancellationToken cancellationToken)
    {
        var existente = await _context.ConfiguracoesEmpresa
            .FirstOrDefaultAsync(cancellationToken);

        if (existente != null)
        {
            existente.Atualizar(request.NomeEmpresa, request.Cnpj, request.Telefone, request.Endereco, request.Email);
        }
        else
        {
            var nova = new ConfiguracaoEmpresa(request.NomeEmpresa, request.Cnpj, request.Telefone);
            nova.Atualizar(request.NomeEmpresa, request.Cnpj, request.Telefone, request.Endereco, request.Email);
            await _context.ConfiguracoesEmpresa.AddAsync(nova, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}