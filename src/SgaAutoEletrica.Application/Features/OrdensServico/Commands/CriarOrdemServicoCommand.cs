using MediatR;

namespace SgaAutoEletrica.Application.Features.OrdensServico.Commands;

public class ItemPecaOSRequest
{
    public Guid PecaId { get; set; }
    public int Quantidade { get; set; }
}

public class ItemServicoOSRequest
{
    public Guid ServicoId { get; set; }
}

public class CriarOrdemServicoCommand : IRequest<Guid>
{
    public Guid ClienteId { get; set; }
    public Guid VeiculoId { get; set; }
    public string? Observacao { get; set; }
    public decimal Desconto { get; set; }
    public bool AprovarIniciar { get; set; } 
    public List<ItemPecaOSRequest> Pecas { get; set; } = new();
    public List<ItemServicoOSRequest> Servicos { get; set; } = new();
}