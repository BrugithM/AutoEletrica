using MediatR;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Domain.Entities;
using SgaAutoEletrica.Domain.ValueObjects;

namespace SgaAutoEletrica.Application.Features.Clientes.Commands;

public class CriarClienteHandler : IRequestHandler<CriarClienteCommand, Guid>
{
    private readonly IClienteRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CriarClienteHandler(IClienteRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CriarClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = new Cliente(request.NomeCompleto, request.Cpf, request.Telefone);

        if (!string.IsNullOrWhiteSpace(request.Logradouro) &&
            !string.IsNullOrWhiteSpace(request.Bairro) &&
            !string.IsNullOrWhiteSpace(request.Cidade) &&
            !string.IsNullOrWhiteSpace(request.Estado) &&
            !string.IsNullOrWhiteSpace(request.Cep))
        {
            var endereco = new Endereco(
                request.Logradouro,
                request.Bairro,
                request.Cidade,
                request.Estado,
                request.Cep,
                request.Numero,
                request.Complemento);
            cliente.AdicionarEndereco(endereco);
        }

        await _repository.Adicionar(cliente, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return cliente.Id;
    }
}