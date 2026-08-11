using MediatR;
using SgaAutoEletrica.Application.Common.Interfaces;

namespace SgaAutoEletrica.Application.Features.Pecas.Commands;

public class MovimentarEstoqueHandler : IRequestHandler<MovimentarEstoqueCommand>
{
    private readonly IPecaRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public MovimentarEstoqueHandler(IPecaRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(MovimentarEstoqueCommand request, CancellationToken cancellationToken)
    {
        var peca = await _repository.ObterPorId(request.PecaId, cancellationToken)
            ?? throw new InvalidOperationException("Peça não encontrada.");

        if (request.Entrada)
            peca.DarEntradaEstoque(request.Quantidade);
        else
            peca.DarBaixaEstoque(request.Quantidade);

        _repository.Atualizar(peca);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}