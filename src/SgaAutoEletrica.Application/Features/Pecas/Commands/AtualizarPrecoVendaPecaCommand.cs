using MediatR;

namespace SgaAutoEletrica.Application.Features.Pecas.Commands;

public class AtualizarPrecoVendaPecaCommand : IRequest
{
    public int Id { get; set; }
    public decimal NovoValorVenda { get; set; }
}