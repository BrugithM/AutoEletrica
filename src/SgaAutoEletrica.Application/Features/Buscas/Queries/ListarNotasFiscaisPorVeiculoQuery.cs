using MediatR;
using SgaAutoEletrica.Application.Features.Buscas.DTOs;

namespace SgaAutoEletrica.Application.Features.Buscas.Queries;

public class ListarNotasFiscaisPorVeiculoQuery : IRequest<List<NotaFiscalSaidaResumoDTO>>
{
    public Guid VeiculoId { get; set; }
}