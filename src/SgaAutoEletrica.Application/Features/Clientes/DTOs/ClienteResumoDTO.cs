namespace SgaAutoEletrica.Application.Features.Clientes.DTOs;

//DTO simplificado para listagens
public class ClienteResumoDTO
{
    public Guid Id {get;set;}
    public string NomeCompleto {get; set;} = string.Empty;
    public string Telefone {get; set;} = string.Empty;
}