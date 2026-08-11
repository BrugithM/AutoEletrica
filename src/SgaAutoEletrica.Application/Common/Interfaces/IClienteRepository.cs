using SgaAutoEletrica.Domain.Entities;

namespace SgaAutoEletrica.Application.Common.Interfaces;

public interface IClienteRepository
{
    Task<Cliente?> ObterPorId(Guid id, CancellationToken cancellationToken=default);
    Task<Cliente?> ObterPorCpf(string cpf, CancellationToken cancellationToken=default);
    Task<List<Cliente>> ListarTodos(CancellationToken cancellationToken=default);
    Task <List<Cliente>> ObterPorNome(string termo, CancellationToken cancellationToken=default);
    Task Adicionar(Cliente cliente, CancellationToken cancellationToken = default);
    void Atualizar(Cliente cliente);
    void Remover(Cliente cliente);
}