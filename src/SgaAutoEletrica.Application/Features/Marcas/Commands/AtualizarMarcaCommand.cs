using MediatR;

namespace SgaAutoEletrica.Application.Features.Marcas.Commands;

public class AtualizarMarcaCommand : IRequest
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
}