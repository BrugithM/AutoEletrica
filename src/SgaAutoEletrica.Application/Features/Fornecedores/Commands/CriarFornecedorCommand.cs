using MediatR;

namespace SgaAutoEletrica.Application.Features.Fornecedores.Commands;

public class CriarFornecedorCommand : IRequest<Guid>
{
    public string NomeEmpresa { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? Contato { get; set; }
}