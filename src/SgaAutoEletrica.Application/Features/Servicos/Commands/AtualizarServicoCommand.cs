using MediatR;

namespace SgaAutoEletrica.Application.Features.Servicos.Commands;

public class AtualizarServicoCommand : IRequest
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public decimal PrecoPadrao { get; set; }
}