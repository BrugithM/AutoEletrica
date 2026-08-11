using MediatR;
using SgaAutoEletrica.Application.Common.Interfaces;

namespace SgaAutoEletrica.Application.Features.OrdensServico.Commands;

public class RemoverItemServicoOSHandler : IRequestHandler<RemoverItemServicoOSCommand>
{
    private readonly IOrdemServicoRepository _osRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoverItemServicoOSHandler(IOrdemServicoRepository osRepository, IUnitOfWork unitOfWork)
    {
        _osRepository = osRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RemoverItemServicoOSCommand request, CancellationToken cancellationToken)
    {
        var os = await _osRepository.ObterPorId(request.OrdemServicoId, cancellationToken)
            ?? throw new InvalidOperationException("OS não encontrada.");

        os.RemoverItemServico(request.ItemId);
        _osRepository.Atualizar(os);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}