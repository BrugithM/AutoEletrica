using MediatR;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Domain.Entities;

namespace SgaAutoEletrica.Application.Features.OrdensServico.Commands;

public class CriarOrdemServicoHandler : IRequestHandler<CriarOrdemServicoCommand, Guid>
{
    private readonly IOrdemServicoRepository _osRepository;
    private readonly IPecaRepository _pecaRepository;
    private readonly IServicoRepository _servicoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CriarOrdemServicoHandler(
        IOrdemServicoRepository osRepository,
        IPecaRepository pecaRepository,
        IServicoRepository servicoRepository,
        IUnitOfWork unitOfWork)
    {
        _osRepository = osRepository;
        _pecaRepository = pecaRepository;
        _servicoRepository = servicoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CriarOrdemServicoCommand request, CancellationToken cancellationToken)
    {
        var proximoNumero = await _osRepository.ObterProximoNumero(cancellationToken);

        var os = new OrdemServico(proximoNumero, request.ClienteId, request.VeiculoId, request.Observacao);

        await _osRepository.Adicionar(os, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return os.Id;
    }
}