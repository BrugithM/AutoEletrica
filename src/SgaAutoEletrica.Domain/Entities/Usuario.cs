using SgaAutoEletrica.Domain.Enums;

namespace SgaAutoEletrica.Domain.Entities;

public class Usuario
{
    public Guid Id{get; private set;}
    public string Nome{get; private set;}
    public string SenhaHash{get; private set;}
    public NivelUsuario Nivel{get; private set;}
    public bool Ativo{get; private set;}
    public DateTime DataCriacao{get; private set;}
    public DateTime? UltimoLogin {get; private set;}

    private Usuario()
    {
        Nome = string.Empty;
        SenhaHash = string.Empty;
    }

    public Usuario(string nome, string senhaHash, NivelUsuario nivel = NivelUsuario.Operador)
    {
        if(string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome é obrigatório", nameof(nome));
        if(string.IsNullOrWhiteSpace(senhaHash))
            throw new ArgumentException("Senha é obrigatória", nameof(senhaHash));

        Id= Guid.NewGuid();
        Nome = nome;
        SenhaHash = senhaHash;
        Nivel=nivel;
        Ativo=true;
        DataCriacao = DateTime.UtcNow;
    }

    public void AtualizarUltimoLogin()
    {
        UltimoLogin = DateTime.UtcNow;
    }

    public void AlterarSenha(string novaSenhaHash)
    {
        if(string.IsNullOrWhiteSpace(novaSenhaHash))
            throw new ArgumentException("Senha é obrigatória", nameof(novaSenhaHash));
        SenhaHash = novaSenhaHash;
    }

    public void AlterarNivel(NivelUsuario novoNivel)
    {
        Nivel = novoNivel;
    }

    public void Desativar() => Ativo = false;
    public void Ativar() => Ativo = true;
}