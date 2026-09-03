using SgaAutoEletrica.Domain.Entities;
using SgaAutoEletrica.Domain.Enums;

namespace SgaAutoEletrica.Application.Common.Interfaces;

public interface ISessaoUsuario
{
    Usuario? UsuarioAtual { get; }
    void DefinirUsuario(Usuario usuario);
    void Limpar();
    bool EhAdministrador { get; }
    bool TemPermissao(NivelUsuario nivelNecessario);
}