namespace SgaAutoEletrica.Domain.Entities;

/// <summary>
/// Representa uma Nota Fiscal de Entrada (compra de mercadoria do fornecedor).
/// Ao ser finalizada, dá entrada no estoque das peças.
/// </summary>
public class NotaFiscalEntrada
{
    public Guid Id { get; private set; }
    public string Numero { get; private set; }
    public DateTime DataEntrada { get; private set; }

    public Guid FornecedorId { get; private set; }
    public Fornecedor Fornecedor { get; private set; } = null!;

    public ICollection<ItemNotaFiscalEntrada> Itens { get; private set; } = new List<ItemNotaFiscalEntrada>();
    public decimal ValorTotal { get; private set; }

    public bool Finalizada { get; private set; }
    public string? Observacao { get; private set; }

    private NotaFiscalEntrada()
    {
        Numero = string.Empty;
    }

    public NotaFiscalEntrada(string numero, Guid fornecedorId, string? observacao = null)
    {
        if (string.IsNullOrWhiteSpace(numero))
            throw new ArgumentException("Número da nota é obrigatório.", nameof(numero));

        Id = Guid.NewGuid();
        Numero = numero;
        FornecedorId = fornecedorId;
        Observacao = observacao;
        DataEntrada = DateTime.UtcNow;
        Finalizada = false;
    }

    public void AdicionarItem(Guid pecaId, int quantidade, decimal valorUnitario)
    {
        if (Finalizada)
            throw new InvalidOperationException("Não é possível adicionar itens a uma nota já finalizada.");

        var item = new ItemNotaFiscalEntrada(pecaId, quantidade, valorUnitario);
        Itens.Add(item);
        RecalcularTotal();
    }

    /// <summary>
    /// Finaliza a nota de entrada.
    /// Retorna a lista de (PecaId, Quantidade) para dar entrada no estoque.
    /// </summary>
    public List<(Guid PecaId, int Quantidade)> Finalizar()
    {
        if (Finalizada)
            throw new InvalidOperationException("Nota já está finalizada.");
        if (!Itens.Any())
            throw new InvalidOperationException("Não é possível finalizar uma nota sem itens.");

        Finalizada = true;

        return Itens.Select(i => (i.PecaId, i.Quantidade)).ToList();
    }

    private void RecalcularTotal()
    {
        ValorTotal = Math.Round(Itens.Sum(i => i.ValorTotal), 2);
    }
}