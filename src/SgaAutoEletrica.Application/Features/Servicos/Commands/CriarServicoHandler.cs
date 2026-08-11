using MediatR;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Domain.Entities;

namespace SgaAutoEletrica.Application.Features.Servicos.Commands;

public class CriarServicoHandler : IRequestHandler<CriarServicoCommand, Guid>
{
    private readonly IServicoRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CriarServicoHandler(IServicoRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CriarServicoCommand request, CancellationToken cancellationToken)
    {
        var servico = new Servico(request.Nome, request.PrecoPadrao, request.Descricao);
        await _repository.Adicionar(servico, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return servico.Id;
    }
}