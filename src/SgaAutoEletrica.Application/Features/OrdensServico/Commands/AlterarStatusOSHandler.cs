using MediatR;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Domain.Enums;

namespace SgaAutoEletrica.Application.Features.OrdensServico.Commands;

public class AlterarStatusOSHandler : IRequestHandler<AlterarStatusOSCommand>
{
    private readonly IOrdemServicoRepository _osRepository;
    private readonly IPecaRepository _pecaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AlterarStatusOSHandler(
        IOrdemServicoRepository osRepository,
        IPecaRepository pecaRepository,
        IUnitOfWork unitOfWork)
    {
        _osRepository = osRepository;
        _pecaRepository = pecaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AlterarStatusOSCommand request, CancellationToken cancellationToken)
    {
        var os = await _osRepository.ObterPorId(request.OrdemServicoId, cancellationToken)
            ?? throw new InvalidOperationException("OS não encontrada.");

        switch (request.NovoStatus)
        {
            case StatusOS.EmAndamento:
                os.IniciarServico();
                break;

            case StatusOS.AguardandoPecas:
                os.AguardarPecas();
                break;

            case StatusOS.Finalizada:
                os.Finalizar();
                break;

            case StatusOS.Cancelada:
                // Devolve todas as peças ao estoque
                foreach (var item in os.ItensPeca)
                {
                    var peca = await _pecaRepository.ObterPorId(item.PecaId, cancellationToken);
                    if (peca != null)
                    {
                        peca.DarEntradaEstoque(item.Quantidade);
                        _pecaRepository.Atualizar(peca);
                    }
                }
                os.Cancelar(request.MotivoCancelamento ?? "Cancelado pelo usuário");
                break;

            default:
                throw new InvalidOperationException($"Status {request.NovoStatus} não permitido.");
        }

        _osRepository.Atualizar(os);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}