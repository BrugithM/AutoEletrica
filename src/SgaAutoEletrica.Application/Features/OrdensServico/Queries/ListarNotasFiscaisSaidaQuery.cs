using MediatR;
using SgaAutoEletrica.Application.Features.OrdensServico.DTOs;

namespace SgaAutoEletrica.Application.Features.OrdensServico.Queries;

public class ListarNotasFiscaisSaidaQuery : IRequest<List<NotaFiscalSaidaDTO>>{}