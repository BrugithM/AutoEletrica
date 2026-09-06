using MediatR;

namespace SgaAutoEletrica.Application.Features.Configuracoes.Commands;

public class SalvarConfiguracaoEmpresaCommand : IRequest
{
    public string NomeEmpresa { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string? Endereco { get; set; }
    public string? Email { get; set; }
}