using MediatR;

namespace SgaAutoEletrica.Application.Features.Pecas.Commands;

public class AtualizarPecaCommand : IRequest
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public decimal ValorCusto { get; set; }
    public decimal ValorVenda { get; set; }
    public decimal Imposto { get; set; }
    public string? CodigoPeca { get; set; }
    public Guid? CategoriaId { get; set; }
    public int? EstoqueMinimo { get; set; }
}