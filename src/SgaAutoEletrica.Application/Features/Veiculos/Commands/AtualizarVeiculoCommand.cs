using MediatR;

namespace SgaAutoEletrica.Application.Features.Veiculos.Commands;

public class AtualizarVeiculoCommand : IRequest
{
    public Guid Id { get; set; }
    public string Modelo { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public int Ano { get; set; }
    public string? Versao { get; set; }
    public string? Motor { get; set; }
    public string? TipoMotor { get; set; }
    public string? Cor { get; set; }
    public string? Observacao { get; set; }
}