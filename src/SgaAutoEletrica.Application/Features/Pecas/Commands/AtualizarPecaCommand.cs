using MediatR;

namespace SgaAutoEletrica.Application.Features.Pecas.Commands;

public class AtualizarPecaCommand : IRequest
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal ValorCusto { get; set; }
    public decimal ValorVenda { get; set; }
    public string? CodigoPeca { get; set; }
    public int? MarcaId { get; set; }
    public Guid? CategoriaId { get; set; }
    public Guid? FornecedorId { get; set; }
    public int? EstoqueMinimo { get; set; }
}