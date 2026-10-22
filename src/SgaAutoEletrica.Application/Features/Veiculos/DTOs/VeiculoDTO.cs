using SgaAutoEletrica.Domain.Enums;

namespace SgaAutoEletrica.Application.Features.Veiculos.DTOs;

public class VeiculoDTO
{
    public Guid Id { get; set; }
    public string Placa { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public Guid? MarcaVeiculoId { get; set; }
    public string? MarcaVeiculoNome { get; set; }
    public int Ano { get; set; }
    public string? Versao { get; set; }
    public string? Motor { get; set; }
    public TipoMotor? TipoMotor { get; set; }
    public string? Cor { get; set; }
    public string? Observacao { get; set; }
    public Guid ClienteId { get; set; }
    public string NomeCliente { get; set; } = string.Empty;
    public string CpfCliente { get; set; } = string.Empty;
    public string TelefoneCliente { get; set; } = string.Empty;
    public bool Ativo { get; set; }
}