using SgaAutoEletrica.Domain.Entities;

namespace SgaAutoEletrica.Application.Common.Interfaces;

public interface IFornecedorRepository
{
    Task<Fornecedor?> ObterPorId(Guid id, CancellationToken cancellationToken = default);
    Task<Fornecedor?> ObterPorCnpj(string cnpj, CancellationToken cancellationToken = default);
    Task<List<Fornecedor>> ListarTodos(CancellationToken cancellationToken = default);
    Task<List<Fornecedor>> BuscarPorNome(string termo, CancellationToken cancellationToken = default);
    Task Adicionar(Fornecedor fornecedor, CancellationToken cancellationToken = default);
    void Atualizar(Fornecedor fornecedor);
    void Remover(Fornecedor fornecedor);
}