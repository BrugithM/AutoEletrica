using MediatR;

namespace SgaAutoEletrica.Application.Features.Fornecedores.Commands;

public class AtualizarItemNotaRequest
{
    public Guid? Id { get; set; }
    public int PecaId { get; set; }
    public decimal Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
}

public class AtualizarNotaFiscalEntradaCommand : IRequest
{
    public Guid Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public Guid FornecedorId { get; set; }
    public DateTime DataEntrada { get; set; }
    public string? Observacao { get; set; }
    public List<AtualizarItemNotaRequest> Itens { get; set; } = new();
}