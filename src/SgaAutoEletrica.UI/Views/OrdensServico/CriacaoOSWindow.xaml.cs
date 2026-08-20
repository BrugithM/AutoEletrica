using System.Windows;
using MediatR;
using SgaAutoEletrica.UI.ViewModels.OrdensServico;

namespace SgaAutoEletrica.UI.Views.OrdensServico;

public partial class CriacaoOSWindow : Window
{
    private readonly CriacaoOSViewModel _viewModel;

    public CriacaoOSWindow(IMediator mediator)
    {
        InitializeComponent();
        _viewModel = new CriacaoOSViewModel(mediator);
        DataContext = _viewModel;
        Loaded += CriacaoOSWindow_Loaded;
    }

    private async void CriacaoOSWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.CarregarDadosAsync();
    }

    private void BtnAdicionarPeca_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.AdicionarPeca();
    }

    private void BtnAdicionarServico_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.AdicionarServico();
    }

    private async void BtnSalvar_Click(object sender, RoutedEventArgs e)
    {
        var sucesso = await _viewModel.SalvarAsync();
        if (sucesso)
            DialogResult = true;
    }
}