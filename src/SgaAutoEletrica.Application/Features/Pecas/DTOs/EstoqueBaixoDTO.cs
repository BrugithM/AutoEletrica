namespace SgaAutoEletrica.Application.Features.Pecas.DTOs;

public class EstoqueBaixoDTO
{
    public Guid Id { get; set; }
    public string IdPeca { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public int Estoque { get; set; }
    public int EstoqueMinimo { get; set; }
}