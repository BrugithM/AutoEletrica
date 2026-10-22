
using MediatR;
using SgaAutoEletrica.Application.Features.MarcasVeiculo.Commands;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.MarcasVeiculo;
public class CriarMarcaVeiculoHandler : IRequestHandler<CriarMarcaVeiculoCommand, Guid>
{
    private readonly AppDbContext _context;
    public CriarMarcaVeiculoHandler(AppDbContext context) => _context = context;

    public async Task<Guid> Handle(CriarMarcaVeiculoCommand request, CancellationToken cancellationToken)
    {
        var marca = new Domain.Entities.MarcaVeiculo(request.Nome);
        _context.MarcasVeiculo.Add(marca);
        await _context.SaveChangesAsync(cancellationToken);
        return marca.Id;
    }
}