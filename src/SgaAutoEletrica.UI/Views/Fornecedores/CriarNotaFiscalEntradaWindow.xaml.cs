using System.Windows;
using MediatR;
using SgaAutoEletrica.UI.ViewModels.Fornecedores;

namespace SgaAutoEletrica.UI.Views.Fornecedores;

public partial class CriarNotaFiscalEntradaWindow : Window
{
    private readonly CriarNotaFiscalEntradaViewModel _viewModel;

    public CriarNotaFiscalEntradaWindow(IMediator mediator)
    {
        InitializeComponent();
        _viewModel = new CriarNotaFiscalEntradaViewModel(mediator);
        DataContext = _viewModel;
        Loaded += CriarNotaFiscalEntradaWindow_Loaded;
    }

    private async void CriarNotaFiscalEntradaWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.CarregarDadosAsync();
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