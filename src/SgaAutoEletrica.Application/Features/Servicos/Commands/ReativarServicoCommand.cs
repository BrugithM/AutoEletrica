using MediatR;

namespace SgaAutoEletrica.Application.Features.Servicos.Commands;

public class ReativarServicoCommand : IRequest
{
    public Guid Id { get; set; }
}