namespace SgaAutoEletrica.Domain.Entities;

public class Servico
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; }
    public string? Descricao { get; private set; }
    public decimal PrecoPadrao { get; private set; }
    public bool Ativo { get; private set; }

    private Servico()
    {
        Nome = string.Empty;
    }

    public Servico(string nome, decimal precoPadrao, string? descricao = null)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome do serviço é obrigatório.", nameof(nome));
        if (precoPadrao < 0)
            throw new ArgumentException("Preço padrão não pode ser negativo.", nameof(precoPadrao));

        Id = Guid.NewGuid();
        Nome = nome;
        PrecoPadrao = precoPadrao;
        Descricao = descricao;
        Ativo = true;
    }

    public void AtualizarPreco(decimal novoPreco)
    {
        if (novoPreco < 0)
            throw new ArgumentException("Preço não pode ser negativo.", nameof(novoPreco));
        PrecoPadrao = novoPreco;
    }

    public void AtualizarDados(string nome, string? descricao = null)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome do serviço é obrigatório.", nameof(nome));

        Nome = nome;
        Descricao = descricao;
    }

    public void Desativar() => Ativo = false;

    public void Ativar() => Ativo = true;
}