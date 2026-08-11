namespace SgaAutoEletrica.Application.Features.Dashboard.DTOs;

public class FaturamentoMensalDTO
{
    public int Mes{get;set;}
    public int Ano{get;set;}
    public decimal TotalPecas{get;set;}
    public decimal TotalServicos{get;set;}
    public decimal TotalGeral{get;set;}
    public decimal QuantidadeOS{get;set;}
}