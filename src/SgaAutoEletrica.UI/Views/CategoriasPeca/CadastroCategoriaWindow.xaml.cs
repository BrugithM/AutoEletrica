using System.Windows;
using MediatR;
using SgaAutoEletrica.UI.ViewModels.CategoriasPeca;

namespace SgaAutoEletrica.UI.Views.CategoriasPeca;

public partial class CadastroCategoriaWindow : Window
{
    private readonly CadastroCategoriaViewModel _viewModel;

    public CadastroCategoriaWindow(IMediator mediator)
    {
        InitializeComponent();
        _viewModel = new CadastroCategoriaViewModel(mediator);
        DataContext = _viewModel;
    }

    private async void BtnSalvar_Click(object sender, RoutedEventArgs e)
    {
        var sucesso = await _viewModel.SalvarAsync();
        if (sucesso)
            DialogResult = true;
    }

    private void BtnLimpar_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.Limpar();
    }
}