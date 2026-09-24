namespace SgaAutoEletrica.Application.Features.Buscas.DTOs;

public class NotaFiscalSaidaDetalheDTO
{
    public Guid Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public DateTime DataEmissao { get; set; }
    public string NomeCliente { get; set; } = string.Empty;
    public string TelefoneCliente { get; set; } = string.Empty;
    public string PlacaVeiculo { get; set; } = string.Empty;
    public string ModeloVeiculo { get; set; } = string.Empty;
    public string MarcaVeiculo { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
    public string? Observacao { get; set; }
    public List<ItemNotaFiscalSaidaDetalheDTO> Itens { get; set; } = new();
}

public class ItemNotaFiscalSaidaDetalheDTO
{
    public string Descricao { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal ValorTotal { get; set; }
}