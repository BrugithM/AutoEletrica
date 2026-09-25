using MediatR;
using SgaAutoEletrica.Application.Common.DTOs;
using SgaAutoEletrica.Application.Features.OrdensServico.DTOs;
using SgaAutoEletrica.Domain.Enums;

namespace SgaAutoEletrica.Application.Features.OrdensServico.Queries;

public class ListarOSQuery : IRequest<ListaPaginadaDTO<OrdemServicoResumoDTO>>
{
    public string? TermoBusca { get; set; }
    public StatusOS? Status { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 20;
}