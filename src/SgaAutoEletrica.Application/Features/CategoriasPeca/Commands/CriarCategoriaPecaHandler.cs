using MediatR;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Domain.Entities;

namespace SgaAutoEletrica.Application.Features.CategoriasPeca.Commands;

public class CriarCategoriaPecaHandler : IRequestHandler<CriarCategoriaPecaCommand, Guid>
{
    private readonly ICategoriaPecaRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CriarCategoriaPecaHandler(ICategoriaPecaRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CriarCategoriaPecaCommand request, CancellationToken cancellationToken)
    {
        var categoria = new CategoriaPeca(request.Nome, request.Descricao);
        await _repository.Adicionar(categoria, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return categoria.Id;
    }
}