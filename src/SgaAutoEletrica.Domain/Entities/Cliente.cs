using SgaAutoEletrica.Domain.ValueObjects;
namespace SgaAutoEletrica.Domain.Entities;

public class Cliente
{
    public Guid Id { get; private set; }
    public string NomeCompleto { get; private set; }
    public CpfCnpj? CpfCnpj { get; private set; }
    public Telefone? Telefone { get; private set; }
    public Endereco? Endereco { get; private set; }
    public DateTime DataCadastro { get; private set; }
    public bool Ativo { get; private set; }

    public ICollection<Veiculo> Veiculos { get; private set; } = new List<Veiculo>();

    private Cliente()
    {
        NomeCompleto = string.Empty;
    }

    public Cliente(string nomeCompleto, string? cpfCnpj, string? telefone)
    {
        if (string.IsNullOrWhiteSpace(nomeCompleto))
            throw new ArgumentException("Nome é obrigatório", nameof(nomeCompleto));

        Id = Guid.NewGuid();
        NomeCompleto = nomeCompleto;
        CpfCnpj = CpfCnpj.Criar(cpfCnpj);
        Telefone = Telefone.Criar(telefone);
        DataCadastro = DateTime.UtcNow;
        Ativo = true;
    }

    public void AtualizarDados(string nomeCompleto, string? cpfCnpj, string? telefone)
    {
        if (string.IsNullOrWhiteSpace(nomeCompleto))
            throw new ArgumentException("Nome é obrigatório", nameof(nomeCompleto));
        NomeCompleto = nomeCompleto;
        CpfCnpj = CpfCnpj.Criar(cpfCnpj);
        Telefone = Telefone.Criar(telefone);
    }

    public void AdicionarEndereco(Endereco endereco)
    {
        Endereco = endereco ?? throw new ArgumentNullException(nameof(endereco));
    }

    public void AdicionarVeiculo(Veiculo veiculo)
    {
        if (veiculo == null)
            throw new ArgumentNullException(nameof(veiculo));

        Veiculos.Add(veiculo);
    }

    public void Desativar() => Ativo = false;
    public void DesativarComVeiculos()
    {
        Ativo = false;
        foreach (var veiculo in Veiculos)
            veiculo.Desativar();
    }
    public void Ativar() => Ativo = true;
}