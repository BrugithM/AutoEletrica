using MediatR;
using SgaAutoEletrica.Application.Common.Interfaces;

namespace SgaAutoEletrica.Application.Features.OrdensServico.Commands;

public class RemoverItemPecaOSHandler : IRequestHandler<RemoverItemPecaOSCommand>
{
    private readonly IOrdemServicoRepository _osRepository;
    private readonly IPecaRepository _pecaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoverItemPecaOSHandler(
        IOrdemServicoRepository osRepository,
        IPecaRepository pecaRepository,
        IUnitOfWork unitOfWork)
    {
        _osRepository = osRepository;
        _pecaRepository = pecaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RemoverItemPecaOSCommand request, CancellationToken cancellationToken)
    {
        var os = await _osRepository.ObterPorId(request.OrdemServicoId, cancellationToken)
            ?? throw new InvalidOperationException("OS não encontrada.");

        // Encontra o item antes de remover para saber qual peça e quantidade
        var item = os.ItensPeca.FirstOrDefault(i => i.Id == request.ItemId)
            ?? throw new InvalidOperationException("Item não encontrado na OS.");

        // Devolve a peça ao estoque
        var peca = await _pecaRepository.ObterPorId(item.PecaId, cancellationToken);
        if (peca != null)
        {
            peca.DarEntradaEstoque(item.Quantidade);
            _pecaRepository.Atualizar(peca);
        }

        os.RemoverItemPeca(request.ItemId);
        _osRepository.Atualizar(os);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}