namespace SgaAutoEletrica.Application.Features.Buscas.DTOs;

public class PecaBuscaDTO
{
    public Guid Id { get; set; }
    public string IdPeca { get; set; } = string.Empty;
    public string? CodigoPeca { get; set; }
    public string? CodigoBarras { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string? CategoriaNome { get; set; }
    public decimal ValorVenda { get; set; }
    public int Estoque { get; set; }
}