namespace SgaAutoEletrica.Domain.Entities;

public class ItemServicoOS
{
     public Guid Id{get; private set;}
    public Guid OrdemServicoId {get; private set;}
    public OrdemServico OrdemServico{get; private set;} = null;

    public Guid ServicoId {get; private set;}
    public Servico Servico{get; private set;}=null;

    public decimal PrecoUnitario{get; private set;}

    private ItemServicoOS(){}

    public ItemServicoOS(Guid servicoId, decimal precoUnitario)
    {
        if(precoUnitario<0)
        throw new ArgumentException("Preco não pode ser negativo.", nameof(precoUnitario));

        Id = Guid.NewGuid();
        ServicoId = servicoId;
        PrecoUnitario = precoUnitario;
    }
}