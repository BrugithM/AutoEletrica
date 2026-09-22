using SgaAutoEletrica.Domain.ValueObjects;

namespace SgaAutoEletrica.Domain.Entities;

public class Fornecedor
{
    public Guid Id{get; private set;}
    public string NomeEmpresa{get; private set;}
    public Cnpj Cnpj {get; private set;}
    public Telefone? Telefone{get; private set;}
    public Endereco? Endereco{get; private set;}
    public string? Contato { get; private set; }
    public DateTime DataCadastro { get; private set; }
    public bool Ativo{get; private set;}

    public ICollection<Peca> Pecas { get; private set; } = new List<Peca>();
    
    public Fornecedor()
    {
        NomeEmpresa = string.Empty;
        Cnpj = null!;
    }

    public Fornecedor(string nomeEmpresa, string cnpj)
    {
        if(string.IsNullOrWhiteSpace(nomeEmpresa))
        throw new ArgumentException("Nome da empresa é obrigatório", nameof(nomeEmpresa));

        Id = Guid.NewGuid(); 
        NomeEmpresa = nomeEmpresa;
        Cnpj = new Cnpj(cnpj);
        DataCadastro = DateTime.UtcNow;
        Ativo = true;
    }

    public void AtualizarDados(string nomeEmpresa, string? telefone = null, string? contato = null)
    {
        if(string.IsNullOrWhiteSpace(nomeEmpresa))
        throw new ArgumentException("Nome da empresa é obrigatório", nameof(nomeEmpresa));
        NomeEmpresa = nomeEmpresa;
        Contato = contato;

        if(!string.IsNullOrWhiteSpace(telefone))
        Telefone = new Telefone(telefone);
    }

    public void AdicionarEndereco(Endereco endereco)
    {
        Endereco = endereco ?? throw new ArgumentNullException(nameof(endereco));
    }

    public void Desativar() => Ativo = false;
    public void Ativar() => Ativo = true;
}