namespace SgaAutoEletrica.Domain.Entities;

public class ConfiguracaoEmpresa
{
    public Guid Id { get; private set; }
    public string NomeEmpresa { get; private set; }
    public string Cnpj { get; private set; }
    public string Telefone { get; private set; }
    public string? Endereco { get; private set; }
    public string? Email { get; private set; }
    public DateTime? UltimaAtualizacao { get; private set; }

    private ConfiguracaoEmpresa()
    {
        NomeEmpresa = string.Empty;
        Cnpj = string.Empty;
        Telefone = string.Empty;
    }

    public ConfiguracaoEmpresa(string nomeEmpresa, string cnpj, string telefone)
    {
        if (string.IsNullOrWhiteSpace(nomeEmpresa))
            throw new ArgumentException("Nome da empresa é obrigatório.", nameof(nomeEmpresa));
        if (string.IsNullOrWhiteSpace(cnpj))
            throw new ArgumentException("CNPJ é obrigatório.", nameof(cnpj));
        if (string.IsNullOrWhiteSpace(telefone))
            throw new ArgumentException("Telefone é obrigatório.", nameof(telefone));

        Id = Guid.NewGuid();
        NomeEmpresa = nomeEmpresa;
        Cnpj = cnpj;
        Telefone = telefone;
        UltimaAtualizacao = DateTime.UtcNow;
    }

    public void Atualizar(string nomeEmpresa, string cnpj, string telefone, string? endereco = null, string? email = null)
    {
        NomeEmpresa = nomeEmpresa;
        Cnpj = cnpj;
        Telefone = telefone;
        Endereco = endereco;
        Email = email;
        UltimaAtualizacao = DateTime.UtcNow;
    }
}