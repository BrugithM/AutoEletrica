namespace SgaAutoEletrica.Domain.Entities;

/// <summary>
/// Representa um item em uma Nota Fiscal de Entrada (compra de fornecedor).
/// </summary>
public class ItemNotaFiscalEntrada
{
    public Guid Id { get; private set; }

    public Guid NotaFiscalEntradaId { get; private set; }
    public NotaFiscalEntrada NotaFiscalEntrada { get; private set; } = null!;

    public Guid PecaId { get; private set; }
    public Peca Peca { get; private set; } = null!;

    public int Quantidade { get; private set; }
    public decimal ValorUnitario { get; private set; }
    public decimal ValorTotal { get; private set; }

    private ItemNotaFiscalEntrada() { }

    public ItemNotaFiscalEntrada(Guid pecaId, int quantidade, decimal valorUnitario)
    {
        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero.", nameof(quantidade));
        if (valorUnitario < 0)
            throw new ArgumentException("Valor unitário não pode ser negativo.", nameof(valorUnitario));

        Id = Guid.NewGuid();
        PecaId = pecaId;
        Quantidade = quantidade;
        ValorUnitario = valorUnitario;
        ValorTotal = Math.Round(quantidade * valorUnitario, 2);
    }
}