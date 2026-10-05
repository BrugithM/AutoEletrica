namespace SgaAutoEletrica.Domain.Entities;
public class CategoriaPeca
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; }
    public string? Descricao { get; private set; }

    public ICollection<Peca> Pecas { get; private set; } = new List<Peca>();

    private CategoriaPeca()
    {
        Nome = string.Empty;
    }

    public CategoriaPeca(string nome, string? descricao = null)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome da categoria é obrigatório.", nameof(nome));

        Id = Guid.NewGuid();
        Nome = nome;
        Descricao = descricao;
    }

    public void Atualizar(string nome, string? descricao = null)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome da categoria é obrigatório.", nameof(nome));

        Nome = nome;
        Descricao = descricao;
    }
}