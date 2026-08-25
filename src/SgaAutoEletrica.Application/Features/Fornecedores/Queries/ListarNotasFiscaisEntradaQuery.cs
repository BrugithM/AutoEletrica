using MediatR;
using SgaAutoEletrica.Application.Features.Fornecedores.DTOs;

namespace SgaAutoEletrica.Application.Features.Fornecedores.Queries;

public class ListarNotasFiscaisEntradaQuery : IRequest<List<NotaFiscalEntradaDTO>> { }