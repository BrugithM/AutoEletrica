using MediatR;
using SgaAutoEletrica.Application.Common.DTOs;
using SgaAutoEletrica.Application.Features.Clientes.DTOs;

namespace SgaAutoEletrica.Application.Features.Clientes.Queries;

public class ListarClientesQuery : IRequest<ListaPaginadaDTO<ClienteDTO>>
{
    public string? TermoBusca { get; set; }
    
    public string? Nome { get; set; }
    public string? Cpf { get; set; }
    public string? Telefone { get; set; }
    public string? Endereco { get; set; }
    
    public bool? Ativo { get; set; } = true;
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 20;
}