using MediatR;
using SgaAutoEletrica.Application.Features.Configuracoes.DTOs;

namespace SgaAutoEletrica.Application.Features.Configuracoes.Queries;

public class ListarConfiguracoesImpressoraQuery : IRequest<List<ConfiguracaoImpressoraDTO>> { }