using System.Windows;
using MediatR;
using SgaAutoEletrica.UI.ViewModels.MarcasVeiculo;

namespace SgaAutoEletrica.UI.Views.MarcasVeiculo;

public partial class CadastroMarcaVeiculoWindow : Window
{
    private readonly CadastroMarcaVeiculoViewModel _viewModel;

    public CadastroMarcaVeiculoWindow(IMediator mediator, Guid? marcaId = null) 
    {
        InitializeComponent();
        _viewModel = new CadastroMarcaVeiculoViewModel(mediator, marcaId);
        DataContext = _viewModel;
    }

    private async void BtnSalvar_Click(object sender, RoutedEventArgs e)
    {
        if (await _viewModel.SalvarAsync())
            DialogResult = true;
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}