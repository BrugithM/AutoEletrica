using MediatR;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Domain.Entities;

namespace SgaAutoEletrica.Application.Features.Pecas.Commands;

public class CriarPecaHandler : IRequestHandler<CriarPecaCommand, Guid>
{
    private readonly IPecaRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CriarPecaHandler(IPecaRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CriarPecaCommand request, CancellationToken cancellationToken)
    {
        var peca = new Peca(
            request.IdPeca,
            request.Nome,
            request.Descricao,
            request.Marca,
            request.ValorCusto,
            request.ValorVenda,
            request.Imposto,
            request.EstoqueInicial,
            request.EstoqueMinimo,
            request.CodigoPeca,
            request.CodigoBarras,
            request.CategoriaId,
            request.FornecedorId);

        await _repository.Adicionar(peca, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return peca.Id;
    }
}