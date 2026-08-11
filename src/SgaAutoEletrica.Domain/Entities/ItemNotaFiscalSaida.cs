namespace SgaAutoEletrica.Domain.Entities;

public class ItemNotaFiscalSaida
{
    public Guid Id { get; private set; }

    public Guid NotaFiscalSaidaId { get; private set; }
    public NotaFiscalSaida NotaFiscalSaida { get; private set; } = null!;

    public string Descricao { get; private set; }
    public int Quantidade { get; private set; }
    public decimal ValorUnitario { get; private set; }
    public decimal ValorTotal { get; private set; }

    private ItemNotaFiscalSaida()
    {
        Descricao = string.Empty;
    }
    
    public ItemNotaFiscalSaida(string descricao, int quantidade, decimal valorUnitario)
    {
        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException("Descrição é obrigatória.", nameof(descricao));
        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero.", nameof(quantidade));
        if (valorUnitario < 0)
            throw new ArgumentException("Valor unitário não pode ser negativo.", nameof(valorUnitario));

        Id = Guid.NewGuid();
        Descricao = descricao;
        Quantidade = quantidade;
        ValorUnitario = valorUnitario;
        ValorTotal = Math.Round(quantidade * valorUnitario, 2);
    }
}