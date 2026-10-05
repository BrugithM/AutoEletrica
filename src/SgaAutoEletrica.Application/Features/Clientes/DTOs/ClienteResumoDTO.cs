namespace SgaAutoEletrica.Application.Features.Clientes.DTOs;

public class ClienteResumoDTO
{
    public Guid Id {get;set;}
    public string NomeCompleto {get; set;} = string.Empty;
    public string Telefone {get; set;} = string.Empty;
}