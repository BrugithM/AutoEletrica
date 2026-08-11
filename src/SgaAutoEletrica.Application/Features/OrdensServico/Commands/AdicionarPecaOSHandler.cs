using MediatR;
using SgaAutoEletrica.Application.Common.Interfaces;

namespace SgaAutoEletrica.Application.Features.OrdensServico.Commands;

public class AdicionarPecaOSHandler : IRequestHandler<AdicionarPecaOSCommand>
{
    private readonly IOrdemServicoRepository _osRepository;
    private readonly IPecaRepository _pecaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AdicionarPecaOSHandler(
        IOrdemServicoRepository osRepository,
        IPecaRepository pecaRepository,
        IUnitOfWork unitOfWork)
    {
        _osRepository = osRepository;
        _pecaRepository = pecaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AdicionarPecaOSCommand request, CancellationToken cancellationToken)
    {
        var os = await _osRepository.ObterPorId(request.OrdemServicoId, cancellationToken)
            ?? throw new InvalidOperationException("OS não encontrada.");

        var peca = await _pecaRepository.ObterPorId(request.PecaId, cancellationToken)
            ?? throw new InvalidOperationException("Peça não encontrada.");

        peca.DarBaixaEstoque(request.Quantidade);
        _pecaRepository.Atualizar(peca);

        // Adiciona o item na OS com o preço de venda atual da peça
        os.AdicionarPeca(request.PecaId, request.Quantidade, peca.ValorVenda);
        _osRepository.Atualizar(os);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}