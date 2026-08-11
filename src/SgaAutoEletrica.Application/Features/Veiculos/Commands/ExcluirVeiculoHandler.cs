using MediatR;
using SgaAutoEletrica.Application.Common.Interfaces;

namespace SgaAutoEletrica.Application.Features.Veiculos.Commands;

public class ExcluirVeiculoHandler : IRequestHandler<ExcluirVeiculoCommand>
{
    private readonly IVeiculoRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ExcluirVeiculoHandler(IVeiculoRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ExcluirVeiculoCommand request, CancellationToken cancellationToken)
    {
        var veiculo = await _repository.ObterPorId(request.Id, cancellationToken)
            ?? throw new InvalidOperationException("Veículo não encontrado.");

        _repository.Remover(veiculo);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}