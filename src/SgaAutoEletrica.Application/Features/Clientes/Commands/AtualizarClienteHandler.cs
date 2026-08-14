using MediatR;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Domain.ValueObjects;

namespace SgaAutoEletrica.Application.Features.Clientes.Commands;

public class AtualizarClienteHandler : IRequestHandler<AtualizarClienteCommand>
{
    private readonly IClienteRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public AtualizarClienteHandler(IClienteRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AtualizarClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = await _repository.ObterPorId(request.Id, cancellationToken)
            ?? throw new InvalidOperationException("Cliente não encontrado.");
        
        cliente.AtualizarDados(request.NomeCompleto, request.Telefone);

        if(
            !string.IsNullOrWhiteSpace(request.Logradouro) &&
            !string.IsNullOrWhiteSpace(request.Bairro) &&
            !string.IsNullOrWhiteSpace(request.Cidade) &&
            !string.IsNullOrWhiteSpace(request.Estado) &&
            !string.IsNullOrWhiteSpace(request.Cep)
        )
        {
            var endereco = new Endereco(
                request.Logradouro,
                request.Bairro,
                request.Cidade,
                request.Estado,
                request.Cep,
                request.Numero,
                request.Complemento
            );
            cliente.AdicionarEndereco(endereco);
        }

        _repository.Atualizar(cliente);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}