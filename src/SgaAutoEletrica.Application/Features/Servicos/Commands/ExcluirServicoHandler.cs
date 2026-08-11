using MediatR;
using SgaAutoEletrica.Application.Common.Interfaces;

namespace SgaAutoEletrica.Application.Features.Servicos.Commands;

public class ExcluirServicoHandler : IRequestHandler<ExcluirServicoCommand>
{
    private readonly IServicoRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ExcluirServicoHandler(IServicoRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ExcluirServicoCommand request, CancellationToken cancellationToken)
    {
        var servico = await _repository.ObterPorId(request.Id, cancellationToken)
            ?? throw new InvalidOperationException("Serviço não encontrado.");

        _repository.Remover(servico);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}