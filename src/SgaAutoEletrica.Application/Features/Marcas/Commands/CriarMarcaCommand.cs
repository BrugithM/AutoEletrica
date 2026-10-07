using MediatR;

namespace SgaAutoEletrica.Application.Features.Marcas.Commands;

public class CriarMarcaCommand : IRequest<int>
{
    public string Nome { get; set; } = string.Empty;
}