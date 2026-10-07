using MediatR;
using SgaAutoEletrica.Application.Common.DTOs;
using SgaAutoEletrica.Application.Features.Pecas.DTOs;

namespace SgaAutoEletrica.Application.Features.Pecas.Queries;

public class ListarPecasQuery : IRequest<ListaPaginadaDTO<PecaDTO>>
{
    public string? TermoBusca { get; set; }

    public string? Nome { get; set; }
    public string? CodigoPeca { get; set; }
    public string? CodigoBarras { get; set; }
    public int? MarcaId { get; set; }
    public Guid? CategoriaId { get; set; }
    public Guid? FornecedorId { get; set; }
    public bool ApenasEstoqueBaixo { get; set; }

    public bool? Ativo { get; set; } = true;
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 20;
}