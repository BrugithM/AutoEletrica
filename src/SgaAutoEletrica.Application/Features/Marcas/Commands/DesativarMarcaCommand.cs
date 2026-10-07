using MediatR;

namespace SgaAutoEletrica.Application.Features.Marcas.Commands;

public class DesativarMarcaCommand : IRequest
{
    public int Id { get; set; }
}