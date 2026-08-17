using System.Windows;
using MediatR;
using SgaAutoEletrica.Application.Features.Pecas.DTOs;
using SgaAutoEletrica.UI.ViewModels.Pecas;

namespace SgaAutoEletrica.UI.Views.Pecas;

public partial class MovimentacaoEstoqueWindow : Window
{
    private readonly MovimentacaoEstoqueViewModel _viewModel;

    public MovimentacaoEstoqueWindow(IMediator mediator, PecaDTO peca)
    {
        InitializeComponent();
        _viewModel = new MovimentacaoEstoqueViewModel(mediator, peca);
        DataContext = _viewModel;
    }

    private async void BtnSalvar_Click(object sender, RoutedEventArgs e)
    {
        var sucesso = await _viewModel.SalvarAsync();
        if (sucesso)
            DialogResult = true;
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}