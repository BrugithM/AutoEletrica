namespace SgaAutoEletrica.Application.Features.Pecas.DTOs;

public class PecaDTO
{
    public Guid Id { get; set; }
    public string IdPeca { get; set; } = string.Empty;
    public string? CodigoPeca { get; set; }
    public string? CodigoBarras { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string? CategoriaNome { get; set; }
    public string? FornecedorNome { get; set; }
    public decimal ValorCusto { get; set; }
    public decimal ValorVenda { get; set; }
    public decimal Imposto { get; set; }
    public int Estoque { get; set; }
    public int EstoqueMinimo { get; set; }
    public bool Ativo { get; set; }
    public decimal MargemLucro { get; set; }
    public decimal PrecoComImposto { get; set; }
    public bool EstoqueBaixo { get; set; }
}