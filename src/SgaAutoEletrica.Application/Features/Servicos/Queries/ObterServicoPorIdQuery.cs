using MediatR;
using SgaAutoEletrica.Application.Features.Servicos.DTOs;

namespace SgaAutoEletrica.Application.Features.Servicos.Queries;

public class ObterServicoPorIdQuery : IRequest<ServicoDTO?>
{
    public Guid Id { get; set; }
}