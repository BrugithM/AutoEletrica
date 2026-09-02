using SgaAutoEletrica.Domain.Entities;
using SgaAutoEletrica.Domain.Enums;

namespace SgaAutoEletrica.Application.Common.Interfaces;

public interface IAuthService
{
    Task<Usuario?> Autenticar(string nome, string senha, CancellationToken cancellationToken=default);
    Task<List<Usuario>>ListarUsuarios(CancellationToken cancellationToken=default);
    Task CriarUsuario(string nome, string senha, NivelUsuario nivel, CancellationToken cancellationToken=default);
    Task AlterarSenha(Guid usuarioId, string novaSenha, CancellationToken cancellationToken=default);
    Task AlterarNivel(Guid usuarioId, NivelUsuario novoNivel, CancellationToken cancellationToken=default);
}