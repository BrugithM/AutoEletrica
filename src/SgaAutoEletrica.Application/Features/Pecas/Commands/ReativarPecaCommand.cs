using MediatR;

namespace SgaAutoEletrica.Application.Features.Pecas.Commands;

public class ReativarPecaCommand : IRequest
{
    public int Id { get; set; }
}