namespace SgaAutoEletrica.Application.Features.Buscas.DTOs;

public class NotaFiscalEntradaResumoDTO
{
    public Guid Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public DateTime DataEntrada { get; set; }
    public string NomeFornecedor { get; set; } = string.Empty;
    public string CnpjFornecedor { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
    public string? Observacao { get; set; }
}