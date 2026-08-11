namespace SgaAutoEletrica.Application.Features.Dashboard.DTOs;

public class ResumoDiarioDTO
{
    public int TotalOSAbertas{get; set;}
    public int TotalOSEmAndamento{get;set;}
    public int TotalOSFinalizadasHoje{get; set;}
    public decimal FaturamentoHoje{get;set;}
    public int ProdutosEstoqueBaixo{get;set;}
}