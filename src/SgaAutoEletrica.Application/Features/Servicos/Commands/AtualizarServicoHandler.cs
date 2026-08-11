using MediatR;
using SgaAutoEletrica.Application.Common.Interfaces;

namespace SgaAutoEletrica.Application.Features.Servicos.Commands;

public class AtualizarServicoHandler : IRequestHandler<AtualizarServicoCommand>
{
    private readonly IServicoRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public AtualizarServicoHandler(IServicoRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AtualizarServicoCommand request, CancellationToken cancellationToken)
    {
        var servico = await _repository.ObterPorId(request.Id, cancellationToken)
            ?? throw new InvalidOperationException("Serviço não encontrado.");

        servico.AtualizarPreco(request.PrecoPadrao);
        servico.AtualizarDados(request.Nome, request.Descricao);
        _repository.Atualizar(servico);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}