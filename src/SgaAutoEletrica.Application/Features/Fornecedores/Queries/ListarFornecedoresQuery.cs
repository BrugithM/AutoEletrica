using MediatR;
using SgaAutoEletrica.Application.Features.Fornecedores.DTOs;

namespace SgaAutoEletrica.Application.Features.Fornecedores.Queries;

public class ListarFornecedoresQuery : IRequest<List<FornecedorDTO>>
{
    public string? TermoBusca { get; set; }
}