using MediatR;

namespace SgaAutoEletrica.Application.Features.Veiculos.Commands;

public class CriarVeiculoCommand : IRequest<Guid>
{
    public string Placa { get; set; } = string.Empty;
    public Guid ClienteId { get; set; }
    
    public string? Modelo { get; set; }
    public string? Marca { get; set; }
    public int? Ano { get; set; }
    public string? Versao { get; set; }
    public string? Motor { get; set; }
    public string? TipoMotor { get; set; }
    public string? Cor { get; set; }
    public string? Observacao { get; set; }
}