using SgaAutoEletrica.Domain.Entities;

namespace SgaAutoEletrica.Application.Common.Interfaces;

public interface IServicoRepository
{
    Task<Servico?> ObterPorId(Guid id, CancellationToken cancellationToken = default);
    Task<List<Servico>> ListarTodos(CancellationToken cancellationToken = default);
    Task<List<Servico>> BuscarPorNome(string termo, CancellationToken cancellationToken=default);
    Task Adicionar(Servico servico, CancellationToken cancellationToken=default);
    void Atualizar(Servico servico);
    void Remover(Servico servico);
}