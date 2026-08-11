using MediatR;
using SgaAutoEletrica.Application.Common.Interfaces;

namespace SgaAutoEletrica.Application.Features.Clientes.Commands;

public class ExcluirClienteHandler : IRequestHandler<ExcluirClienteCommand>
{
    private readonly IClienteRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ExcluirClienteHandler(IClienteRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ExcluirClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = await _repository.ObterPorId(request.Id, cancellationToken)
            ?? throw new InvalidOperationException("Cliente não encontrado.");

        _repository.Remover(cliente);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}