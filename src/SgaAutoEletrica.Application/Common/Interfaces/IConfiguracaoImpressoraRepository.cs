using SgaAutoEletrica.Domain.Entities;
using SgaAutoEletrica.Domain.Enums;

namespace SgaAutoEletrica.Application.Common.Interfaces;

public interface  IConfiguracaoImpressoraRepository
{
    Task<ConfiguracaoImpressora?> ObterPorTipo(TipoImpressao tipo, CancellationToken cancellationToken=default);
    Task<List<ConfiguracaoImpressora>> ListarTodas(CancellationToken cancellationToken=default);
    Task Adicionar(ConfiguracaoImpressora config, CancellationToken cancellationToken=default);
    void Atualizar(ConfiguracaoImpressora config);
    void Remover(ConfiguracaoImpressora config);
}