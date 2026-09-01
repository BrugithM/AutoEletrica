using SgaAutoEletrica.Domain.Enums;

namespace SgaAutoEletrica.Domain.Entities;

public class ConfiguracaoImpressora
{
    public Guid Id{get;private set;}
    public TipoImpressao Tipo{get;private set;}
    public string NomeImpressora{get;private set;}
    public TamanhoPapel TamanhoPapel{get;private set;}
    public int? Copias{get;private set;}
     public string? MargemSuperior { get; private set; }
    public string? MargemInferior { get; private set; }
    public string? MargemEsquerda { get; private set; }
    public string? MargemDireita { get; private set; }
    public bool Ativo { get; private set; }

    private ConfiguracaoImpressora()
    {
        NomeImpressora = string.Empty;
    }

    public ConfiguracaoImpressora(
        TipoImpressao tipo,
        string nomeImpressora,
        TamanhoPapel tamanhoPapel = TamanhoPapel.A4,
      int? copias = 1,
        string? margemSuperior = null,
        string? margemInferior = null,
        string? margemEsquerda = null,
        string? margemDireita = null)
    {
        if (string.IsNullOrWhiteSpace(nomeImpressora))
            throw new ArgumentException("Nome da impressora é obrigatório.", nameof(nomeImpressora));

        Id = Guid.NewGuid();
        Tipo = tipo;
        NomeImpressora = nomeImpressora;
        TamanhoPapel = tamanhoPapel;
        Copias = copias ?? 1;
        MargemSuperior = margemSuperior;
        MargemInferior = margemInferior;
        MargemEsquerda = margemEsquerda;
        MargemDireita = margemDireita;
        Ativo = true;
    }

    public void Atualizar(
        string nomeImpressora,
        TamanhoPapel tamanhoPapel,
        int? copias = 1,
        string? margemSuperior = null,
        string? margemInferior = null,
        string? margemEsquerda = null,
        string? margemDireita = null)
    {
        if (string.IsNullOrWhiteSpace(nomeImpressora))
            throw new ArgumentException("Nome da impressora é obrigatório.", nameof(nomeImpressora));

        NomeImpressora = nomeImpressora;
        TamanhoPapel = tamanhoPapel;
        Copias = copias ?? 1;
        MargemSuperior = margemSuperior;
        MargemInferior = margemInferior;
        MargemEsquerda = margemEsquerda;
        MargemDireita = margemDireita;
    }

    public void Desativar()=>Ativo =false;

    public void Ativar()=> Ativo = true;

}