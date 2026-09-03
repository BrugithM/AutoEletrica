using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Domain.Entities;
using SgaAutoEletrica.Domain.Enums;

namespace SgaAutoEletrica.Infrastructure.Services;

public class SessaoUsuario : ISessaoUsuario
{
    public Usuario? UsuarioAtual {get; private set;}
    public bool EhAdministrador => UsuarioAtual?.Nivel == NivelUsuario.Administrador;

    public void DefinirUsuario(Usuario usuario)
    {
        UsuarioAtual = usuario;
    }

    public void Limpar()
    {
        UsuarioAtual = null;
    }

    public bool TemPermissao(NivelUsuario nivelNecessario)
    {
        if(UsuarioAtual == null)
            return false;
        if(nivelNecessario == NivelUsuario.Administrador)
            return UsuarioAtual.Nivel == NivelUsuario.Administrador;

        return true;
    }
}