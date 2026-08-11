using MediatR;
using SgaAutoEletrica.Application.Common.Interfaces;

namespace SgaAutoEletrica.Application.Features.OrdensServico.Commands;

public class AdicionarServicoOSHandler : IRequestHandler<AdicionarServicoOSCommand>
{
    private readonly IOrdemServicoRepository _osRepository;
    private readonly IServicoRepository _servicoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AdicionarServicoOSHandler(
        IOrdemServicoRepository osRepository,
        IServicoRepository servicoRepository,
        IUnitOfWork unitOfWork)
    {
        _osRepository = osRepository;
        _servicoRepository = servicoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AdicionarServicoOSCommand request, CancellationToken cancellationToken)
    {
        var os = await _osRepository.ObterPorId(request.OrdemServicoId, cancellationToken)
            ?? throw new InvalidOperationException("OS não encontrada.");

        var servico = await _servicoRepository.ObterPorId(request.ServicoId, cancellationToken)
            ?? throw new InvalidOperationException("Serviço não encontrado.");

        // Usa preço manual se informado, senão usa o preço padrão do serviço
        var preco = request.PrecoManual ?? servico.PrecoPadrao;

        os.AdicionarServico(request.ServicoId, preco);
        _osRepository.Atualizar(os);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}