using MediatR;
using SgaAutoEletrica.Application.Common.Interfaces;

namespace SgaAutoEletrica.Application.Features.Fornecedores.Commands;

public class ExcluirFornecedorHandler : IRequestHandler<ExcluirFornecedorCommand>
{
    private readonly IFornecedorRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ExcluirFornecedorHandler(IFornecedorRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ExcluirFornecedorCommand request, CancellationToken cancellationToken)
    {
        var fornecedor = await _repository.ObterPorId(request.Id, cancellationToken)
            ?? throw new InvalidOperationException("Fornecedor não encontrado.");

        _repository.Remover(fornecedor);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}