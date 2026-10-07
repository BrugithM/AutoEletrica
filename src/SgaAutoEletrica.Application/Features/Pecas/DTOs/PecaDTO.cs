namespace SgaAutoEletrica.Application.Features.Pecas.DTOs;

public class PecaDTO
{
    public int Id { get; set; }
    public string? CodigoPeca { get; set; }
    public string? CodigoBarras { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    
    public int? MarcaId { get; set; }
    public string? MarcaNome { get; set; }
    
    public Guid? CategoriaId { get; set; }
    public string? CategoriaNome { get; set; }
    
    public Guid? FornecedorId { get; set; }
    public string? FornecedorNome { get; set; }
    public string? FornecedorCnpj { get; set; }
    public string? FornecedorTelefone { get; set; }
    public string? FornecedorContato { get; set; }
    
    public decimal ValorCusto { get; set; }
    public decimal ValorVenda { get; set; }
    public decimal MarkupPercentual { get; set; }
    public decimal MargemLucro { get; set; }
    
    public int Estoque { get; set; }
    public int EstoqueMinimo { get; set; }
    public bool Ativo { get; set; }
    public bool EstoqueBaixo { get; set; }
}