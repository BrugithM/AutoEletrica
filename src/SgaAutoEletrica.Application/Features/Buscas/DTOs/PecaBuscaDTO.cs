namespace SgaAutoEletrica.Application.Features.Buscas.DTOs;

public class PecaBuscaDTO
{
    public int Id { get; set; }
    public string? CodigoPeca { get; set; }
    public string? CodigoBarras { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? MarcaNome { get; set; }
    public string? CategoriaNome { get; set; }
    public decimal ValorVenda { get; set; }
    public int Estoque { get; set; }
}