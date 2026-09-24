using SgaAutoEletrica.Domain.Enums;

namespace SgaAutoEletrica.Domain.Entities;
//conecta cliente, veiculo, pecas e servico
public class OrdemServico
{
    public Guid Id { get; private set; }
    public int Numero { get; private set; }

    public Guid ClienteId { get; private set; }
    public Cliente Cliente { get; private set; } = null!;

    public Guid VeiculoId { get; private set; }
    public Veiculo Veiculo { get; private set; } = null!;

    public StatusOS Status { get; private set; }
    public DateTime DataAbertura { get; private set; }
    public DateTime? DataFinalizacao { get; private set; }
    public string? Observacao { get; private set; }
    public decimal Desconto { get; private set; }

    public ICollection<ItemPecaOS> ItensPeca { get; private set; } = new List<ItemPecaOS>();
    public ICollection<ItemServicoOS> ItensServico { get; private set; } = new List<ItemServicoOS>();

    public decimal ValorTotalPecas { get; private set; }
    public decimal ValorTotalServicos { get; private set; }
    public decimal ValorTotal { get; private set; }

    private OrdemServico() { }

    public OrdemServico(int numero, Guid clienteId, Guid veiculoId, string? observacao = null)
    {
        if (numero <= 0)
            throw new ArgumentException("Número da OS deve ser maior que zero.", nameof(numero));

        Id = Guid.NewGuid();
        Numero = numero;
        ClienteId = clienteId;
        VeiculoId = veiculoId;
        Observacao = observacao;
        Status = StatusOS.Aberta;
        DataAbertura = DateTime.UtcNow;
    }

    public void AdicionarPeca(Guid pecaId, int quantidade, decimal precoUnitario)
    {
        if (Status == StatusOS.Finalizada || Status == StatusOS.Cancelada)
            throw new InvalidOperationException("Não é possivel adicionar peças a uma OS finalizada ou cancelada");

        var item = new ItemPecaOS(pecaId, quantidade, precoUnitario);
        ItensPeca.Add(item);
        RecalcularTotais();
    }
    public void AdicionarServico(Guid servicoId, decimal precoUnitario)
    {
        if (Status == StatusOS.Finalizada || Status == StatusOS.Cancelada)
            throw new InvalidOperationException("Não é possível adicionar serviços a uma OS finalizada ou cancelada.");

        var item = new ItemServicoOS(servicoId, precoUnitario);
        ItensServico.Add(item);
        RecalcularTotais();
    }

    public void RemoverItemPeca(Guid itemId)
    {
        if (Status == StatusOS.Finalizada || Status == StatusOS.Cancelada)
            throw new InvalidOperationException("Não é possível remover itens de uma OS finalizada ou cancelada.");

        var item = ItensPeca.FirstOrDefault(i => i.Id == itemId)
            ?? throw new InvalidOperationException("Item não encontrado nesta OS.");

        ItensPeca.Remove(item);
        RecalcularTotais();
    }

    public void RemoverItemServico(Guid itemId)
    {
        if (Status == StatusOS.Finalizada || Status == StatusOS.Cancelada)
            throw new InvalidOperationException("Não é possível remover itens de uma OS finalizada ou cancelada.");

        var item = ItensServico.FirstOrDefault(i => i.Id == itemId)
            ?? throw new InvalidOperationException("Serviço não encontrado nesta OS.");

        ItensServico.Remove(item);
        RecalcularTotais();
    }

    public void RecalcularTotais()
    {
        ValorTotalPecas = Math.Round(ItensPeca.Sum(i => i.ValorTotal), 2);
        ValorTotalServicos = Math.Round(ItensServico.Sum(i => i.PrecoUnitario), 2);

        var subtotal = ValorTotalPecas + ValorTotalServicos;
        ValorTotal = Math.Round(subtotal - Desconto, 2);
    }

    public void AplicarDesconto(decimal desconto)
    {
        if(desconto<0)
            throw new ArgumentException("Desconto não pode ser negativo", nameof(desconto));

        var subtotal = ValorTotalPecas + ValorTotalServicos;
        if(desconto > subtotal)
            throw new ArgumentException("Desconto não pode ser maior que o subtotal", nameof(desconto));

        Desconto = desconto;
        RecalcularTotais();
    }

    public void AtualizarTotais(decimal totalPecas, decimal totalServicos)
{
    ValorTotalPecas = totalPecas;
    ValorTotalServicos = totalServicos;
    var subtotal = totalPecas + totalServicos;
    ValorTotal = Math.Round(subtotal-Desconto,2);
}

    //Transições de status
    public void IniciarServico()
    {
        if (Status != StatusOS.Aberta && Status != StatusOS.AguardandoPecas)
            throw new InvalidOperationException($"Não é possível iniciar uma OS com status {Status}.");

        Status = StatusOS.EmAndamento;
    }

    public void AguardarPecas()
    {
        if (Status != StatusOS.EmAndamento)
            throw new InvalidOperationException($"Não é possível colocar em espera uma OS com status {Status}.");

        Status = StatusOS.AguardandoPecas;
    }

    public void Finalizar()
    {
        if (Status == StatusOS.Finalizada)
            throw new InvalidOperationException("OS já está finalizada.");
        if (Status == StatusOS.Cancelada)
            throw new InvalidOperationException("Não é possível finalizar uma OS cancelada.");
        if (!ItensPeca.Any() && !ItensServico.Any())
            throw new InvalidOperationException("Não é possível finalizar uma OS sem itens.");

        Status = StatusOS.Finalizada;
        DataFinalizacao = DateTime.UtcNow;
    }

    public void Cancelar(string motivo)
    {
        if (Status == StatusOS.Finalizada)
            throw new InvalidOperationException("Não é possível cancelar uma OS já finalizada.");
        if (Status == StatusOS.Cancelada)
            throw new InvalidOperationException("OS já está cancelada.");
        if (string.IsNullOrWhiteSpace(motivo))
            throw new ArgumentException("Motivo do cancelamento é obrigatório.", nameof(motivo));

        Status = StatusOS.Cancelada;
        Observacao = string.IsNullOrWhiteSpace(Observacao)
            ? $"CANCELADA: {motivo}"
            : $"{Observacao} | CANCELADA: {motivo}";
        DataFinalizacao = DateTime.UtcNow;
    }

    public void AdicionarObservacao(string observacao)
    {
        Observacao = observacao;
    }

    
}