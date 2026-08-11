using MediatR;
using SgaAutoEletrica.Application.Features.Fornecedores.DTOs;

namespace SgaAutoEletrica.Application.Features.Fornecedores.Queries;

public class ObterFornecedorPorIdQuery : IRequest<FornecedorDTO?>
{
    public Guid Id { get; set; }
}