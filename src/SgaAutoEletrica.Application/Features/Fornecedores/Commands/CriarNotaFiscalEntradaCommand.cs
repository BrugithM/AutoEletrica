using MediatR;

namespace SgaAutoEletrica.Application.Features.Fornecedores.Commands;

public class ItemNotaEntradaRequest
{
    public Guid PecaId { get; set; }
    public int Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
}

public class CriarNotaFiscalEntradaCommand : IRequest<Guid>
{
    public string Numero { get; set; } = string.Empty;
    public Guid FornecedorId { get; set; }
    public DateTime DataEntrada { get; set; } = DateTime.Now;
    public List<ItemNotaEntradaRequest> Itens { get; set; } = new();
    public string? Observacao { get; set; }
}