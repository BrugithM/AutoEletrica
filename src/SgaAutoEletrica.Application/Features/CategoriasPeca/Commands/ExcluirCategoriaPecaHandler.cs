using MediatR;
using SgaAutoEletrica.Application.Common.Interfaces;

namespace SgaAutoEletrica.Application.Features.CategoriasPeca.Commands;

public class ExcluirCategoriaPecaHandler : IRequestHandler<ExcluirCategoriaPecaCommand>
{
    private readonly ICategoriaPecaRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ExcluirCategoriaPecaHandler(ICategoriaPecaRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ExcluirCategoriaPecaCommand request, CancellationToken cancellationToken)
    {
        var categoria = await _repository.ObterPorId(request.Id, cancellationToken)
            ?? throw new InvalidOperationException("Categoria não encontrada.");

        _repository.Remover(categoria);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}