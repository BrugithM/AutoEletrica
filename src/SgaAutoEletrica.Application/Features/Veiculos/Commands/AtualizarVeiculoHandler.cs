using MediatR;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Domain.Enums;

namespace SgaAutoEletrica.Application.Features.Veiculos.Commands;

public class AtualizarVeiculoHandler : IRequestHandler<AtualizarVeiculoCommand>
{
    private readonly IVeiculoRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public AtualizarVeiculoHandler(IVeiculoRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AtualizarVeiculoCommand request, CancellationToken cancellationToken)
    {
        var veiculo = await _repository.ObterPorId(request.Id, cancellationToken)
            ?? throw new InvalidOperationException("Veículo não encontrado.");

        TipoMotor? tipoMotor = null;
        if (!string.IsNullOrWhiteSpace(request.TipoMotor) &&
            Enum.TryParse<TipoMotor>(request.TipoMotor, out var parsed))
        {
            tipoMotor = parsed;
        }

        veiculo.AtualizarDados(request.Modelo, request.Marca, request.Ano,
            request.Versao, request.Motor, tipoMotor, request.Cor, request.Observacao);

        _repository.Atualizar(veiculo);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}