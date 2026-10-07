namespace SgaAutoEletrica.Domain.Entities;

public class ItemPecaOS
{
    public Guid Id { get; private set; }
    public Guid OrdemServicoId { get; private set; }
    public OrdemServico OrdemServico { get; private set; } = null!;

    public int PecaId { get; private set; }
    public Peca Peca { get; private set; } = null!;

    public int Quantidade { get; private set; }
    public decimal PrecoUnitario { get; private set; }
    public decimal ValorTotal { get; private set; }

    private ItemPecaOS() { }

    public ItemPecaOS(int pecaId, int quantidade, decimal precoUnitario)
    {
        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero.", nameof(quantidade));
        if (precoUnitario < 0)
            throw new ArgumentException("Preço unitário não pode ser negativo.", nameof(precoUnitario));

        Id = Guid.NewGuid();
        PecaId = pecaId;
        Quantidade = quantidade;
        PrecoUnitario = precoUnitario;
        ValorTotal = Math.Round(quantidade * precoUnitario, 2);
    }

    public void DefinirOrdemServico(Guid ordemServicoId)
    {
        OrdemServicoId = ordemServicoId;
    }

    public void AtualizarPrecoUnitario(decimal novoPreco)
    {
        if (novoPreco < 0)
            throw new ArgumentException("Preço unitário não pode ser negativo.", nameof(novoPreco));

        PrecoUnitario = novoPreco;
        ValorTotal = Math.Round(Quantidade * novoPreco, 2);
    }
}