using MediatR;

namespace SgaAutoEletrica.Application.Features.Marcas.Commands;

public class ExcluirMarcaCommand : IRequest
{
    public int Id { get; set; }
}