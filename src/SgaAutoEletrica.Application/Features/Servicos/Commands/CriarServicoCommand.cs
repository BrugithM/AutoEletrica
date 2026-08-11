using MediatR;

namespace SgaAutoEletrica.Application.Features.Servicos.Commands;

public class CriarServicoCommand : IRequest<Guid>
{
    public string Nome {get; set;} =string.Empty;
    public string? Descricao {get; set;}
    public decimal PrecoPadrao {get; set;}    
}