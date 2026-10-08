using MediatR;

namespace SgaAutoEletrica.Application.Features.OrdensServico.Commands;

public class AtualizarQuilometragemOSCommand : IRequest
{
    public Guid OrdemServicoId { get; set; }
    public int? Quilometragem { get; set; }
}