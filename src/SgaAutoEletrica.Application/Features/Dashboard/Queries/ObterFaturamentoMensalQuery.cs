using MediatR;
using SgaAutoEletrica.Application.Features.Dashboard.DTOs;

namespace SgaAutoEletrica.Application.Features.Dashboard.Queries;

public class ObterFaturamentoMensalQuery : IRequest<List<FaturamentoMensalDTO>>
{
    public int Mes {get;set;}
    public int Ano {get;set;}
}