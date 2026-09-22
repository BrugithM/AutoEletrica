namespace SgaAutoEletrica.Application.Features.Fornecedores.DTOs;

public class FornecedorDTO
{
    public Guid Id { get; set; }
    public string NomeEmpresa { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? Contato { get; set; }
    public bool Ativo{get; set;}
}