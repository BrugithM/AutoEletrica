using MediatR;
using SgaAutoEletrica.Application.Common.Interfaces;

namespace SgaAutoEletrica.Application.Features.Pecas.Commands;

public class AtualizarPrecoVendaPecaHandler : IRequestHandler<AtualizarPrecoVendaPecaCommand>
{
    private readonly IPecaRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public AtualizarPrecoVendaPecaHandler(IPecaRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AtualizarPrecoVendaPecaCommand request, CancellationToken cancellationToken)
    {
        var peca = await _repository.ObterPorId(request.Id, cancellationToken)
            ?? throw new InvalidOperationException("Peça não encontrada.");

        peca.AtualizarValorVenda(request.NovoValorVenda);
        _repository.Atualizar(peca);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}