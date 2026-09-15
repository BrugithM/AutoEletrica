namespace SgaAutoEletrica.Application.Features.Veiculos.DTOs;

public class VeiculoDTO
{
    public Guid Id { get; set; }
    public string Placa { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public int Ano { get; set; }
    public string? Versao { get; set; }
    public string? Motor { get; set; }
    public string? TipoMotor { get; set; }
    public string? Cor { get; set; }
    public string? Observacao { get; set; }
    public Guid ClienteId { get; set; }
    public string NomeCliente { get; set; } = string.Empty;
    public string CpfCliente { get; set; } = string.Empty;
    public string TelefoneCliente { get; set; } = string.Empty;
}