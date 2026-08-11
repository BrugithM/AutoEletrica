using MediatR;

namespace SgaAutoEletrica.Application.Features.Pecas.Commands;

public class AtualizarPrecoVendaPecaCommand : IRequest
{
    public Guid Id { get; set; }
    public decimal NovoValorVenda { get; set; }
}