namespace SgaAutoEletrica.Application.Features.Configuracoes.DTOs;

public class ConfiguracaoEmpresaDTO
{
    public Guid Id { get; set; }
    public string NomeEmpresa { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string? Endereco { get; set; }
    public string? Email { get; set; }
    public string? LogoPath { get; set; }
}