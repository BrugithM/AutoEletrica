using MediatR;

namespace SgaAutoEletrica.Application.Features.Fornecedores.Commands;

public class DesativarFornecedorCommand : IRequest
{
    public Guid Id { get; set; }
}