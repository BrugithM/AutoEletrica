using MediatR;

namespace SgaAutoEletrica.Application.Features.Pecas.Commands;

public class ReativarPecaCommand : IRequest
{
    public Guid Id { get; set; }
}