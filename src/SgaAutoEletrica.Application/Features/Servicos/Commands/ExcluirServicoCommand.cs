using MediatR;

namespace SgaAutoEletrica.Application.Features.Servicos.Commands;

public class ExcluirServicoCommand : IRequest
{
    public Guid Id { get; set; }
}