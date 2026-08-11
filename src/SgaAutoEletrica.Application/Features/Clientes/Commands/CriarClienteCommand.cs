using MediatR;

namespace SgaAutoEletrica.Application.Features.Clientes.Commands;

public class CriarClienteCommand : IRequest<Guid>
{
    public string NomeCompleto{get; set;} = string.Empty;
    public string Cpf {get; set;} = string.Empty;
    public string Telefone {get; set;} = string.Empty;

    public string? Logradouro {get; set;}
    public string? Numero {get; set;}
    public string? Complemento {get; set;}
    public string? Bairro {get; set;}
    public string? Cidade {get; set;}
    public string? Estado {get; set;}
    public string? Cep {get; set;}
}