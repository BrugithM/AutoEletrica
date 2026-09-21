using MediatR;

namespace SgaAutoEletrica.Application.Features.Servicos.Commands;

public class DesativarServicoCommand : IRequest
{
    public Guid Id { get; set; }
}