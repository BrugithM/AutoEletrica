using SgaAutoEletrica.Domain.Entities;

namespace SgaAutoEletrica.Application.Common.Interfaces;

public interface IPecaRepository
{
    Task<Peca?> ObterPorId(Guid id, CancellationToken cancellationToken = default);
    Task<Peca?> ObterPorIdPeca(string idPeca, CancellationToken cancellationToken = default);
    Task<Peca?> ObterPorCodigo(string codigo, CancellationToken cancellationToken = default);
    Task<Peca?> ObterPorCodigoBarras(string codigoBarras, CancellationToken cancellationToken = default);
    Task<List<Peca>> ListarTodas(CancellationToken cancellationToken = default);
    Task<List<Peca>> ListarEstoqueBaixo(CancellationToken cancellationToken = default);
    Task<List<Peca>> BuscarPorNome(string termo, CancellationToken cancellationToken = default);
    Task Adicionar(Peca peca, CancellationToken cancellationToken = default);
    void Atualizar(Peca peca);
    void Remover(Peca peca);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}