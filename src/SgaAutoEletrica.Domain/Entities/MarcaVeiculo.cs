namespace SgaAutoEletrica.Domain.Entities;

public class MarcaVeiculo
{
    public Guid Id {get; private set;}
    public string Nome {get; private set;}
    public bool Ativo {get; private set;}

    private MarcaVeiculo() {Nome = string.Empty;}
    
    public MarcaVeiculo(string nome)
    {
        if(string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome da marca é obrigatório.", nameof(nome));

        Id = Guid.NewGuid();
        Nome = nome.Trim();
        Ativo=true;
    }

    public void AtualizarNome(string nome)
    {
        if(string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome da marca é obrigatório.", nameof(nome));
        Nome = nome.Trim();
    }

    public void Desativar() => Ativo = false;
    public void Ativar() => Ativo = true;
    }