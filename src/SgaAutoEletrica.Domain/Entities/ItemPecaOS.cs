namespace SgaAutoEletrica.Domain.Entities;

public class ItemPecaOS
{
    public Guid Id{get; private set;}
    public Guid OrdemServicoId {get; private set;}
    public OrdemServico OrdemServico{get; private set;} = null;

    public Guid PecaId{get; private set;}
    public Peca Peca{get; private set;}

    public int Quantidade { get; private set; }
    public decimal PrecoUnitario { get; private set; }
    public decimal ValorTotal { get; private set; }

    private ItemPecaOS(){}

    public ItemPecaOS(Guid pecaId, int quantidade, decimal precoUnitario)
    {
        if(quantidade <=0)
        throw new ArgumentException("Quantidade deve ser maior que zero.", nameof(quantidade));
        if(PrecoUnitario<0)
        throw new ArgumentException("Preco não pode ser negativo.", nameof(precoUnitario));

        Id = Guid.NewGuid();
        PecaId = pecaId;
        Quantidade = quantidade;
        PrecoUnitario = precoUnitario;
        ValorTotal = Math.Round(quantidade * precoUnitario, 2);
    }
}