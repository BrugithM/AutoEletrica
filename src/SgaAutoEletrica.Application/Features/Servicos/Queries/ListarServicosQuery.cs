using MediatR;
using SgaAutoEletrica.Application.Features.Servicos.DTOs;

namespace SgaAutoEletrica.Application.Features.Servicos.Queries;

public class ListarServicosQuery : IRequest<List<ServicoDTO>>
{
    public string? TermoBusca { get; set; }
}