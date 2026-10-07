namespace SgaAutoEletrica.Application.Features.Pecas.DTOs;

public class EstoqueBaixoDTO
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? MarcaNome { get; set; }
    public int Estoque { get; set; }
    public int EstoqueMinimo { get; set; }
}