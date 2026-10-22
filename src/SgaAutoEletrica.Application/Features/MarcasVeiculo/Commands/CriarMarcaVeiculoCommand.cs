using MediatR;

namespace SgaAutoEletrica.Application.Features.MarcasVeiculo.Commands;
public class CriarMarcaVeiculoCommand : IRequest<Guid>
{
    public string Nome { get; set; } = string.Empty;
}