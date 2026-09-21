using MediatR;

namespace SgaAutoEletrica.Application.Features.Pecas.Commands;

public class DesativarPecaCommand : IRequest
{
    public Guid Id { get; set; }
}