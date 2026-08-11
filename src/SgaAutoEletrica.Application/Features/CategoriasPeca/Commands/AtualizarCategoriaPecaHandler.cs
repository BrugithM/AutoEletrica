using MediatR;
using SgaAutoEletrica.Application.Common.Interfaces;

namespace SgaAutoEletrica.Application.Features.CategoriasPeca.Commands;

public class AtualizarCategoriaPecaHandler : IRequestHandler<AtualizarCategoriaPecaCommand>
{
    private readonly ICategoriaPecaRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public AtualizarCategoriaPecaHandler(ICategoriaPecaRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AtualizarCategoriaPecaCommand request, CancellationToken cancellationToken)
    {
        var categoria = await _repository.ObterPorId(request.Id, cancellationToken)
            ?? throw new InvalidOperationException("Categoria não encontrada.");

        categoria.Atualizar(request.Nome, request.Descricao);
        _repository.Atualizar(categoria);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}