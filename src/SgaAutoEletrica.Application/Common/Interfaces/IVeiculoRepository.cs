using System.ComponentModel;
using SgaAutoEletrica.Domain.Entities;

namespace SgaAutoEletrica.Application.Common.Interfaces;

public interface IVeiculoRepository
{
    Task<Veiculo?> ObterPorId (Guid id, CancellationToken cancellationToken=default);
    Task<Veiculo?> ObterPorPlaca(string placa,CancellationToken cancellationToken=default);
    Task <List<Veiculo>> ListarPorCliente(Guid clienteId, CancellationToken cancellationToken=default);
    Task Adicionar(Veiculo veiculo, CancellationToken cancellationToken=default);
    void Atualizar(Veiculo veiculo);
    void Remover(Veiculo veiculo);

    //ASK : n devia ter busca por modelo?
}