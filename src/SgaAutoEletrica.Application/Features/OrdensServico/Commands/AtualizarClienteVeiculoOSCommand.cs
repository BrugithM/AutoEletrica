using MediatR;

namespace SgaAutoEletrica.Application.Features.OrdensServico.Commands;

public class AtualizarClienteVeiculoOSCommand : IRequest
{
    public Guid OrdemServicoId { get; set; }
    public Guid ClienteId { get; set; }
    public Guid? VeiculoId { get; set; }
}