namespace SgaAutoEletrica.Domain.Entities;

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

    private NotaFiscalEntrada() { Numero = string.Empty; }

    public NotaFiscalEntrada(string numero, Guid fornecedorId, DateTime? dataEntrada = null, string? observacao = null)
    {
        if (string.IsNullOrWhiteSpace(numero)) throw new ArgumentException("Número da nota é obrigatório.", nameof(numero));
        Id = Guid.NewGuid();
        Numero = numero;
        FornecedorId = fornecedorId;
        Observacao = observacao;
        DataEntrada = dataEntrada ?? DateTime.UtcNow;
        Finalizada = false;
    }

    public void AtualizarCabecalho(string numero, Guid fornecedorId, DateTime dataEntrada, string? observacao)
    {
        if (string.IsNullOrWhiteSpace(numero)) throw new ArgumentException("Número da nota é obrigatório.", nameof(numero));
        Numero = numero;
        FornecedorId = fornecedorId;
        DataEntrada = dataEntrada;
        Observacao = observacao;
    }

    public void AdicionarItem(int pecaId, decimal quantidade, decimal valorUnitario)
    {
        var item = new ItemNotaFiscalEntrada(pecaId, quantidade, valorUnitario);
        Itens.Add(item);
        RecalcularTotal();
    }

    public void RemoverItem(Guid itemId)
    {
        var item = Itens.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            Itens.Remove(item);
            RecalcularTotal();
        }
    }

    public List<(int PecaId, decimal Quantidade, decimal QuantidadeAntiga)> Finalizar()
    {
        Finalizada = true;
        return Itens.Select(i => (i.PecaId, i.Quantidade, 0m)).ToList(); 
    }

    public void RecalcularTotal()
    {
        ValorTotal = Math.Round(Itens.Sum(i => i.ValorTotal), 2);
    }
}