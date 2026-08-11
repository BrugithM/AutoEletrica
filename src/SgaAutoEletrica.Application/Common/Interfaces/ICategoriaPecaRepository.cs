using SgaAutoEletrica.Domain.Entities;

namespace SgaAutoEletrica.Application.Common.Interfaces;

public interface ICategoriaPecaRepository
{
    Task<CategoriaPeca?> ObterPorId(Guid id, CancellationToken cancellationToken = default);
    Task<List<CategoriaPeca>> ListarTodas(CancellationToken cancellationToken = default);
    Task Adicionar (CategoriaPeca categoria, CancellationToken cancellationToken=default);
    void Atualizar(CategoriaPeca categoria);
    void Remover(CategoriaPeca categoria);
}