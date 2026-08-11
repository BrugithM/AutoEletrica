using MediatR;
using SgaAutoEletrica.Application.Common.Interfaces;

namespace SgaAutoEletrica.Application.Features.Pecas.Commands;

public class AtualizarPecaHandler : IRequestHandler<AtualizarPecaCommand>
{
    private readonly IPecaRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public AtualizarPecaHandler(IPecaRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AtualizarPecaCommand request, CancellationToken cancellationToken)
    {
        var peca = await _repository.ObterPorId(request.Id, cancellationToken)
            ?? throw new InvalidOperationException("Peça não encontrada.");

        peca.AtualizarDados(
            request.Nome,
            request.Descricao,
            request.Marca,
            request.ValorCusto,
            request.Imposto,
            request.CodigoPeca,
            request.CategoriaId,
            request.EstoqueMinimo);

        _repository.Atualizar(peca);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}