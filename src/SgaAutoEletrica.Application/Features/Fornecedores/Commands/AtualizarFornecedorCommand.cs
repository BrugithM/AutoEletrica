using MediatR;

namespace SgaAutoEletrica.Application.Features.Fornecedores.Commands;

public class AtualizarFornecedorCommand : IRequest
{
    public Guid Id { get; set; }
    public string NomeEmpresa { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? Contato { get; set; }
}