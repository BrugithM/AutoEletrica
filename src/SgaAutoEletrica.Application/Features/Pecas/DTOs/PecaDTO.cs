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
    public Guid? CategoriaId { get; set; }
    
    // Fornecedor
    public Guid? FornecedorId { get; set; }
    public string? FornecedorNome { get; set; }
    public string? FornecedorCnpj { get; set; }
    public string? FornecedorTelefone { get; set; }
    public string? FornecedorContato { get; set; }
    
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