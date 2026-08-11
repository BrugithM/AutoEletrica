namespace SgaAutoEletrica.Domain.Entities;

/// <summary>
/// Categoria ou grupo de peças.
/// O usuário pode criar e gerenciar livremente.
/// </summary>
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

    //Cria uma nova categoria de peças.
    public CategoriaPeca(string nome, string? descricao = null)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome da categoria é obrigatório.", nameof(nome));

        Id = Guid.NewGuid();
        Nome = nome;
        Descricao = descricao;
    }

    // Atualiza o nome e descrição da categoria.
    public void Atualizar(string nome, string? descricao = null)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome da categoria é obrigatório.", nameof(nome));

        Nome = nome;
        Descricao = descricao;
    }
}