using MediatR;

namespace SgaAutoEletrica.Application.Features.Veiculos.Commands;

public class ReativarVeiculoCommand : IRequest
{
    public Guid Id { get; set; }
}