using MediatR;
using SgaAutoEletrica.Application.Features.Configuracoes.DTOs;

namespace SgaAutoEletrica.Application.Features.Configuracoes.Queries;

public class ObterConfiguracaoEmpresaQuery : IRequest<ConfiguracaoEmpresaDTO?>
{
}