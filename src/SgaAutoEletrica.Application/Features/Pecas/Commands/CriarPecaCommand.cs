using MediatR;

namespace SgaAutoEletrica.Application.Features.Pecas.Commands;

public class CriarPecaCommand : IRequest<Guid>
{
    public string IdPeca { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public decimal ValorCusto { get; set; }
    public decimal ValorVenda { get; set; }
    public decimal Imposto { get; set; }
    public int EstoqueInicial { get; set; }
    public int EstoqueMinimo { get; set; } = 5;
    public string? CodigoPeca { get; set; }
    public string? CodigoBarras { get; set; }
    public Guid? CategoriaId { get; set; }
    public Guid? FornecedorId { get; set; }
}