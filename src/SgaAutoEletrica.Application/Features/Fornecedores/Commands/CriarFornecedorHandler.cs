using MediatR;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Domain.Entities;

namespace SgaAutoEletrica.Application.Features.Fornecedores.Commands;

public class CriarFornecedorHandler : IRequestHandler<CriarFornecedorCommand, Guid>
{
    private readonly IFornecedorRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CriarFornecedorHandler(IFornecedorRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CriarFornecedorCommand request, CancellationToken cancellationToken)
    {
        var fornecedor = new Fornecedor(request.NomeEmpresa, request.Cnpj);
        
        if (!string.IsNullOrWhiteSpace(request.Telefone) || !string.IsNullOrWhiteSpace(request.Contato))
            fornecedor.AtualizarDados(request.NomeEmpresa, request.Telefone, request.Contato);

        await _repository.Adicionar(fornecedor, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return fornecedor.Id;
    }
}