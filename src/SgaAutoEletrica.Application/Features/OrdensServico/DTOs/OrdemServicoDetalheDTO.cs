using SgaAutoEletrica.Domain.Enums;

namespace SgaAutoEletrica.Application.Features.OrdensServico.DTOs;

public class OrdemServicoDetalheDTO
{
    public Guid Id { get; set; }
    public int Numero { get; set; }
    public StatusOS Status { get; set; }
    public string? Observacao { get; set; }
    public DateTime DataAbertura { get; set; }
    public DateTime? DataFinalizacao { get; set; }

    public Guid ClienteId { get; set; }
    public string NomeCliente { get; set; } = string.Empty;
    public string TelefoneCliente { get; set; } = string.Empty;

    public Guid VeiculoId { get; set; }
    public string PlacaVeiculo { get; set; } = string.Empty;
    public string ModeloVeiculo { get; set; } = string.Empty;
    public string MarcaVeiculo { get; set; } = string.Empty;
    public int AnoVeiculo { get; set; }

    public decimal ValorTotalPecas { get; set; }
    public decimal ValorTotalServicos { get; set; }
    public decimal Desconto{get; set;}
    public decimal ValorTotal { get; set; }

    public List<ItemPecaOSDTO> ItensPeca { get; set; } = new();
    public List<ItemServicoOSDTO> ItensServico { get; set; } = new();
}

public class ItemPecaOSDTO
{
    public Guid Id { get; set; }
    public Guid PecaId { get; set; }
    public string NomePeca { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal ValorTotal { get; set; }
}

public class ItemServicoOSDTO
{
    public Guid Id { get; set; }
    public Guid ServicoId { get; set; }
    public string NomeServico { get; set; } = string.Empty;
    public decimal PrecoUnitario { get; set; }
}