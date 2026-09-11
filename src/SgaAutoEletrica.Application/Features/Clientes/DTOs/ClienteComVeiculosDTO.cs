namespace SgaAutoEletrica.Application.Features.Clientes.DTOs;

public class ClienteComVeiculosDTO
{
    public Guid Id { get; set; }
    public string NomeCompleto { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string? EnderecoCompleto { get; set; }
    public List<VeiculoResumoDTO> Veiculos { get; set; } = new();
}

public class VeiculoResumoDTO
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
}