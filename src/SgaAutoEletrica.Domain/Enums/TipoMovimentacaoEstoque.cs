namespace SgaAutoEletrica.Domain.Enums;

public enum TipoMovimentacaoEstoque
{
    Entrada = 1,        // Compra de fornecedor
    SaidaVenda = 2,     // Peça usada em uma OS
    SaidaManual = 3,    // Perda, avaria, ajuste
    Devolucao = 4       // Devolução de cliente
}