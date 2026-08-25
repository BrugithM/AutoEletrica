namespace SgaAutoEletrica.Application.Features.OrdensServico.DTOs;

public class NotaFiscalSaidaDTO
{
    public Guid Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public DateTime DataEmissao { get; set; }
    public string NomeCliente { get; set; } = string.Empty;
    public string PlacaVeiculo { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
    public string? Observacao { get; set; }
}