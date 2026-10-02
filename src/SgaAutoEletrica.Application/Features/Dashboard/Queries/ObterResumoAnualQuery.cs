using MediatR;
using SgaAutoEletrica.Application.Features.Dashboard.DTOs;

namespace SgaAutoEletrica.Application.Features.Dashboard.Queries;

public class ObterResumoAnualQuery : IRequest<ResumoAnualDTO>
{
    public int Ano { get; set; }
}