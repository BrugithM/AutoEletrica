using MediatR;

namespace SgaAutoEletrica.Application.Features.Fornecedores.Commands;

public class ExcluirFornecedorCommand : IRequest
{
    public Guid Id { get; set; }
}