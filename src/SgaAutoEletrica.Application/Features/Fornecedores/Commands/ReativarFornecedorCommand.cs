using MediatR;

namespace SgaAutoEletrica.Application.Features.Fornecedores.Commands;

public class ReativarFornecedorCommand : IRequest
{
    public Guid Id { get; set; }
}