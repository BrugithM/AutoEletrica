using MediatR;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Domain.Entities;
using SgaAutoEletrica.Domain.Enums;

namespace SgaAutoEletrica.Application.Features.Veiculos.Commands;

public class CriarVeiculoHandler : IRequestHandler<CriarVeiculoCommand, Guid>
{
    private readonly IVeiculoRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CriarVeiculoHandler(IVeiculoRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CriarVeiculoCommand request, CancellationToken cancellationToken)
    {
        Veiculo veiculo;

        if (!string.IsNullOrWhiteSpace(request.Modelo) &&
            !string.IsNullOrWhiteSpace(request.Marca) &&
            request.Ano.HasValue)
        {
            veiculo = new Veiculo(request.Placa, request.ClienteId, request.Modelo, request.Marca, request.Ano.Value);
            
            if (!string.IsNullOrWhiteSpace(request.Versao) ||
                !string.IsNullOrWhiteSpace(request.Motor) ||
                !string.IsNullOrWhiteSpace(request.Cor))
            {
                TipoMotor? tipoMotor = null;
                if (!string.IsNullOrWhiteSpace(request.TipoMotor) &&
                    Enum.TryParse<TipoMotor>(request.TipoMotor, out var parsed))
                {
                    tipoMotor = parsed;
                }

                veiculo.AtualizarDados(request.Modelo, request.Marca, request.Ano.Value,
                    request.Versao, request.Motor, tipoMotor, request.Cor, request.Observacao);
            }
        }
        else
        {
            veiculo = new Veiculo(request.Placa, request.ClienteId);
        }

        await _repository.Adicionar(veiculo, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return veiculo.Id;
    }
}