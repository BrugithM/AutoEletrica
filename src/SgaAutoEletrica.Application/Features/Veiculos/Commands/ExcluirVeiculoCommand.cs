using MediatR;

namespace SgaAutoEletrica.Application.Features.Veiculos.Commands;

public class ExcluirVeiculoCommand : IRequest
{
    public Guid Id {get;set;}
}