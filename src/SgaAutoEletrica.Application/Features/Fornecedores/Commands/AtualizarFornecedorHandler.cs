using MediatR;
using SgaAutoEletrica.Application.Common.Interfaces;

namespace SgaAutoEletrica.Application.Features.Fornecedores.Commands;

public class AtualizarFornecedorHandler : IRequestHandler<AtualizarFornecedorCommand>
{
    private readonly IFornecedorRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public AtualizarFornecedorHandler(IFornecedorRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AtualizarFornecedorCommand request, CancellationToken cancellationToken)
    {
        var fornecedor = await _repository.ObterPorId(request.Id, cancellationToken)
            ?? throw new InvalidOperationException("Fornecedor não encontrado.");

        fornecedor.AtualizarDados(request.NomeEmpresa, request.Telefone, request.Contato);
        _repository.Atualizar(fornecedor);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}