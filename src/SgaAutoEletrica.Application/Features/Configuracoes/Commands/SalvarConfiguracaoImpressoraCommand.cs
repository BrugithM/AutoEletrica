using MediatR;
using SgaAutoEletrica.Domain.Enums;

namespace SgaAutoEletrica.Application.Features.Configuracoes.Commands;

public class SalvarConfiguracaoImpressoraCommand : IRequest
{
    public TipoImpressao Tipo { get; set; }
    public string NomeImpressora { get; set; } = string.Empty;
    public TamanhoPapel TamanhoPapel { get; set; }
    public int? Copias { get; set; }
    public string? MargemSuperior { get; set; }
    public string? MargemInferior { get; set; }
    public string? MargemEsquerda { get; set; }
    public string? MargemDireita { get; set; }
}