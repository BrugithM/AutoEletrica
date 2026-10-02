namespace SgaAutoEletrica.Application.Features.Dashboard.DTOs;

public class ResumoMensalDTO
{
    public int Mes { get; set; }
    public int Ano { get; set; }
    public decimal FaturamentoTotal { get; set; }
    public int OSAbertas { get; set; }
    public int OSFinalizadas { get; set; }
    public decimal TicketMedio { get; set; }
    public int ProdutosEstoqueBaixo { get; set; }
    public int OSAguardandoPecas { get; set; }
}