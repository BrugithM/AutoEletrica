using MediatR;

namespace SgaAutoEletrica.Application.Features.Veiculos.Commands;

public class DesativarVeiculoCommand : IRequest
{
    public Guid Id { get; set; }
}