using MediatR;
using SgaAutoEletrica.Application.Features.Dashboard.DTOs;

namespace SgaAutoEletrica.Application.Features.Dashboard.Queries;

public class ObterResumoDiarioQuery : IRequest<ResumoDiarioDTO>{}