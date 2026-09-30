using SgaAutoEletrica.Application.Features.OrdensServico.DTOs;
using SgaAutoEletrica.Application.Features.Pecas.DTOs;

namespace SgaAutoEletrica.Application.Common.Interfaces;

public interface IImpressaoService
{
    void ImprimirOS(OrdemServicoDetalheDTO os);
    void ImprimirNotaFiscal(OrdemServicoDetalheDTO os, string numeroNota);
    void ImprimirCupomFiscal(OrdemServicoDetalheDTO os);
    void ImprimirEtiqueta(PecaDTO peca);
    System.Drawing.Bitmap GerarEtiquetaBitmap(PecaDTO peca, string nomeEmpresa);
}