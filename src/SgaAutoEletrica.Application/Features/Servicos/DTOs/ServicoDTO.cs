namespace SgaAutoEletrica.Application.Features.Servicos.DTOs;

public class ServicoDTO
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public decimal PrecoPadrao { get; set; }
    public bool Ativo { get; set; }
}