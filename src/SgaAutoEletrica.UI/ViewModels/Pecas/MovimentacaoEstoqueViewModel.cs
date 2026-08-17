using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using MediatR;
using SgaAutoEletrica.Application.Features.Pecas.Commands;
using SgaAutoEletrica.Application.Features.Pecas.DTOs;

namespace SgaAutoEletrica.UI.ViewModels.Pecas;

public class MovimentacaoEstoqueViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;
    private readonly PecaDTO _peca;

    public string NomePeca { get; }
    public int EstoqueAtual { get; }

    public int Quantidade { get; set; }
    public bool EhEntrada { get; set; } = true;

    public MovimentacaoEstoqueViewModel(IMediator mediator, PecaDTO peca)
    {
        _mediator = mediator;
        _peca = peca;
        NomePeca = peca.Nome;
        EstoqueAtual = peca.Estoque;
    }

    public async Task<bool> SalvarAsync()
    {
        if (Quantidade <= 0)
        {
            MessageBox.Show("Quantidade deve ser maior que zero.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        await _mediator.Send(new MovimentarEstoqueCommand
        {
            PecaId = _peca.Id,
            Quantidade = Quantidade,
            Entrada = EhEntrada
        });

        return true;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}