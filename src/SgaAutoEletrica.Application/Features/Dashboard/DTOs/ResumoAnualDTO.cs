namespace SgaAutoEletrica.Application.Features.Dashboard.DTOs;

public class ResumoAnualDTO
{
    public int Ano { get; set; }
    public decimal FaturamentoTotal { get; set; }
    public int OSFinalizadas { get; set; }
    public decimal TicketMedio { get; set; }
}