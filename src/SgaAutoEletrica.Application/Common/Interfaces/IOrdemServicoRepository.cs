using SgaAutoEletrica.Domain.Entities;
using SgaAutoEletrica.Domain.Enums;

namespace SgaAutoEletrica.Application.Common.Interfaces;

public interface IOrdemServicoRepository
{
    Task<OrdemServico?> ObterPorId(Guid id, CancellationToken cancellationToken = default);
    Task<OrdemServico?> ObterPorNumero(int numero, CancellationToken cancellationToken = default);
    Task<List<OrdemServico>> ListarTodas(CancellationToken cancellationToken = default);
    Task<List<OrdemServico>> ListarPorCliente(Guid clienteId, CancellationToken cancellationToken = default);
    Task<List<OrdemServico>> ListarPorVeiculo(Guid veiculoId, CancellationToken cancellationToken = default);
    Task<List<OrdemServico>> ListarPorStatus(StatusOS status, CancellationToken cancellationToken = default);
    Task<int> ObterProximoNumero(CancellationToken cancellationToken = default);
    Task Adicionar(OrdemServico os, CancellationToken cancellationToken = default);
    void Atualizar(OrdemServico os);
}