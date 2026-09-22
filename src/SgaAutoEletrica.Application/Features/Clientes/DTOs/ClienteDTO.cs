namespace SgaAutoEletrica.Application.Features.Clientes.DTOs;

public class ClienteDTO
{
    public Guid Id { get; set; }
    public string NomeCompleto { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string? EnderecoCompleto { get; set; }
    public DateTime DataCadastro { get; set; }
    public int QuantidadeVeiculos { get; set; }
    public bool Ativo { get; set; }
}