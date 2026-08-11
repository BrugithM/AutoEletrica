using MediatR;
using SgaAutoEletrica.Application.Common.Interfaces;

namespace SgaAutoEletrica.Application.Features.Pecas.Commands;

public class ExcluirPecaHandler : IRequestHandler<ExcluirPecaCommand>
{
    private readonly IPecaRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ExcluirPecaHandler(IPecaRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ExcluirPecaCommand request, CancellationToken cancellationToken)
    {
        var peca = await _repository.ObterPorId(request.Id, cancellationToken)
            ?? throw new InvalidOperationException("Peça não encontrada.");

        _repository.Remover(peca);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}