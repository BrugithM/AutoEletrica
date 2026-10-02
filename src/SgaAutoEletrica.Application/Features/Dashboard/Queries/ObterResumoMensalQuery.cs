using MediatR;
using SgaAutoEletrica.Application.Features.Dashboard.DTOs;

namespace SgaAutoEletrica.Application.Features.Dashboard.Queries;

public class ObterResumoMensalQuery : IRequest<ResumoMensalDTO>
{
    public int Mes { get; set; }
    public int Ano { get; set; }
}