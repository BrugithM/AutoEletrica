using SgaAutoEletrica.Application.Features.OrdensServico.DTOs;

namespace SgaAutoEletrica.Application.Common.Interfaces;

public interface IImpressaoService
{
    void ImprimirOS(OrdemServicoDetalheDTO os);
    void ImprimirNotaFiscal(OrdemServicoDetalheDTO os, string numeroNota);
    void ImprimirCupomFiscal(OrdemServicoDetalheDTO os);
}