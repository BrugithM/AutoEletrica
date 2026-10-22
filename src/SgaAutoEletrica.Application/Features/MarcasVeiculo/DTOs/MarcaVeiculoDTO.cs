namespace SgaAutoEletrica.Application.Features.MarcasVeiculo.DTOs;

public class MarcaVeiculoDTO
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public bool Ativo { get; set; }
}