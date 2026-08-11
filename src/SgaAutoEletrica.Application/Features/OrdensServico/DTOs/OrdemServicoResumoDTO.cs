using SgaAutoEletrica.Domain.Enums;

namespace SgaAutoEletrica.Application.Features.OrdensServico.DTOs;

public class OrdemServicoResumoDTO
{
    public Guid Id { get; set; }
    public int Numero { get; set; }
    public string NomeCliente { get; set; } = string.Empty;
    public string PlacaVeiculo { get; set; } = string.Empty;
    public string ModeloVeiculo { get; set; } = string.Empty;
    public StatusOS Status { get; set; }
    public decimal ValorTotal { get; set; }
    public DateTime DataAbertura { get; set; }
    public DateTime? DataFinalizacao { get; set; }
}