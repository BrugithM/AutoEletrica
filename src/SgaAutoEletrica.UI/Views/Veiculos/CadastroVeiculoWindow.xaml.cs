using System.Windows;
using MediatR;
using SgaAutoEletrica.UI.ViewModels.Veiculos;

namespace SgaAutoEletrica.UI.Views.Veiculos;

public partial class CadastroVeiculoWindow : Window
{
    private readonly CadastroVeiculoViewModel _viewModel;

    public CadastroVeiculoWindow(IMediator mediator, Guid? veiculoId = null, Guid? clienteIdPreSelecionado = null)
    {
        InitializeComponent();
        _viewModel = new CadastroVeiculoViewModel(mediator, veiculoId, clienteIdPreSelecionado);
        DataContext = _viewModel;
        Loaded += CadastroVeiculoWindow_Loaded;
    }

    private async void CadastroVeiculoWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.CarregarClientesAsync();
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