namespace SgaAutoEletrica.Domain.Entities;

public class Marca
{
    public int Id { get; private set; }
    public string Nome { get; private set; }
    public bool Ativo { get; private set; }

    private Marca()
    {
        Nome = string.Empty;
    }

    public Marca(string nome, string? descricao = null)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome da marca é obrigatório.", nameof(nome));

        Nome = nome;
        Ativo = true;
    }

    public void Atualizar(string nome, string? descricao = null)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome da marca é obrigatório.", nameof(nome));

        Nome = nome;
    }

    public void Desativar() => Ativo = false;
    public void Ativar() => Ativo = true;
}