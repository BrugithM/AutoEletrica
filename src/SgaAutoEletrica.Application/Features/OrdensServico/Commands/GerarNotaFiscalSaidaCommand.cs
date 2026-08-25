using MediatR;

namespace SgaAutoEletrica.Application.Features.OrdensServico.Commands;

public class GerarNotaFiscalSaidaCommand : IRequest<Guid>
{
    public Guid OrdemServicoId {get;set;}
    public string NumeroNota{get; set;} = string.Empty;
}