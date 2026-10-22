using MediatR;

namespace SgaAutoEletrica.Application.Features.MarcasVeiculo.Commands;

public class AtualizarMarcaVeiculoCommand : IRequest
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
}

public class AlternarAtivoMarcaVeiculoCommand : IRequest
{
    public Guid Id { get; set; }
    public bool Ativo { get; set; }
}