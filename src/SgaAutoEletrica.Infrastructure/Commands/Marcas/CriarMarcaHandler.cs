using MediatR;
using SgaAutoEletrica.Application.Features.Marcas.Commands;
using SgaAutoEletrica.Domain.Entities;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.Marcas;

public class CriarMarcaHandler : IRequestHandler<CriarMarcaCommand, int>
{
    private readonly AppDbContext _context;

    public CriarMarcaHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CriarMarcaCommand request, CancellationToken cancellationToken)
    {
        var marca = new Marca(request.Nome);
        await _context.Marcas.AddAsync(marca, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return marca.Id;
    }
}