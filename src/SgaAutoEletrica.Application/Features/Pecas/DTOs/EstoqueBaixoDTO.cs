namespace SgaAutoEletrica.Application.Features.Pecas.DTOs;

public class EstoqueBaixoDTO
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? MarcaNome { get; set; }
    public decimal Estoque { get; set; }
    public decimal EstoqueMinimo { get; set; }
}