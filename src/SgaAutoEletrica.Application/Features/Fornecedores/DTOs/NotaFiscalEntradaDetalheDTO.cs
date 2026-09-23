namespace SgaAutoEletrica.Application.Features.Fornecedores.DTOs;

public class NotaFiscalEntradaDetalheDTO
{
    public Guid Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public DateTime DataEntrada { get; set; }
    public string NomeFornecedor { get; set; } = string.Empty;
    public string CnpjFornecedor { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
    public string? Observacao { get; set; }
    public List<ItemNotaEntradaDetalheDTO> Itens { get; set; } = new();
}

public class ItemNotaEntradaDetalheDTO
{
    public string NomePeca { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal ValorTotal { get; set; }
}