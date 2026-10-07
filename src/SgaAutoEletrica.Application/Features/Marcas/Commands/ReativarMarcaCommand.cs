using MediatR;

namespace SgaAutoEletrica.Application.Features.Marcas.Commands;

public class ReativarMarcaCommand : IRequest
{
    public int Id { get; set; }
}