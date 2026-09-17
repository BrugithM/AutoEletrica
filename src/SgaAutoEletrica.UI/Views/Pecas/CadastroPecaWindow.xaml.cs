using System.Windows;
using MediatR;
using SgaAutoEletrica.UI.ViewModels.Pecas;

namespace SgaAutoEletrica.UI.Views.Pecas;

public partial class CadastroPecaWindow : Window
{
    private readonly CadastroPecaViewModel _viewModel;

    public CadastroPecaWindow(IMediator mediator, Guid? pecaId = null)
    {
        InitializeComponent();
        _viewModel = new CadastroPecaViewModel(mediator, pecaId);
        DataContext = _viewModel;
        Loaded += CadastroPecaWindow_Loaded;
    }

    private async void CadastroPecaWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.CarregarDadosAuxiliaresAsync();
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