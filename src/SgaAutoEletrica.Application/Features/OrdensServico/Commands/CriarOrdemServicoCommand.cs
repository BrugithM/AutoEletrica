using MediatR;

namespace SgaAutoEletrica.Application.Features.OrdensServico.Commands;

public class CriarOrdemServicoCommand : IRequest<Guid>
{
    public Guid ClienteId { get; set; }
    public Guid VeiculoId { get; set; }
    public string? Observacao { get; set; }
}