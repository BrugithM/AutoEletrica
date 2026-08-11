using SgaAutoEletrica.Domain.Enums;

namespace SgaAutoEletrica.Domain.Entities;

/// <summary>
/// Representa uma Nota Fiscal de Saída emitida para o cliente.
/// Pode ser gerada a partir de uma Ordem de Serviço finalizada.
/// </summary>
public class NotaFiscalSaida
{
    public Guid Id { get; private set; }
    public string Numero { get; private set; }
    public DateTime DataEmissao { get; private set; }

    public Guid? OrdemServicoId { get; private set; }
    public OrdemServico? OrdemServico { get; private set; }

    public Guid ClienteId { get; private set; }
    public Cliente Cliente { get; private set; } = null!;
    public Guid VeiculoId { get; private set; }
    public Veiculo Veiculo { get; private set; } = null!;

    public ICollection<ItemNotaFiscalSaida> Itens { get; private set; } = new List<ItemNotaFiscalSaida>();
    public decimal ValorTotal { get; private set; }

    public string? Observacao { get; private set; }

    private NotaFiscalSaida()
    {
        Numero = string.Empty;
    }

    public NotaFiscalSaida(string numero, OrdemServico ordemServico, string? observacao = null)
    {
        if (string.IsNullOrWhiteSpace(numero))
            throw new ArgumentException("Número da nota é obrigatório.", nameof(numero));
        if (ordemServico == null)
            throw new ArgumentNullException(nameof(ordemServico));
        if (ordemServico.Status != StatusOS.Finalizada)
            throw new InvalidOperationException("Só é possível emitir NF para OS finalizada.");

        Id = Guid.NewGuid();
        Numero = numero;
        DataEmissao = DateTime.UtcNow;
        OrdemServicoId = ordemServico.Id;
        ClienteId = ordemServico.ClienteId;
        VeiculoId = ordemServico.VeiculoId;
        Observacao = observacao;
    }

    public void AdicionarItem(string descricao, int quantidade, decimal valorUnitario)
    {
        var item = new ItemNotaFiscalSaida(descricao, quantidade, valorUnitario);
        Itens.Add(item);
        RecalcularTotal();
    }

    private void RecalcularTotal()
    {
        ValorTotal = Math.Round(Itens.Sum(i => i.ValorTotal), 2);
    }
}