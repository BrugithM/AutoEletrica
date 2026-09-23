using MediatR;

namespace SgaAutoEletrica.Application.Features.Fornecedores.Queries;

public class VerificarNFExistenteQuery : IRequest<bool>
{
    public string Numero { get; set; } = string.Empty;
    public Guid FornecedorId { get; set; }
}