using System.Windows;
using System.Windows.Input;
using MediatR;
using SgaAutoEletrica.UI.ViewModels.Veiculos;

namespace SgaAutoEletrica.UI.Views.Veiculos;

public partial class OSVinculadasWindow : Window
{
    private readonly OSVinculadasViewModel _viewModel;

    public OSVinculadasWindow(IMediator mediator, Guid veiculoId, string placaVeiculo)
    {
        InitializeComponent();
        _viewModel = new OSVinculadasViewModel(mediator, veiculoId, placaVeiculo);
        DataContext = _viewModel;
        Loaded += OSVinculadasWindow_Loaded;
    }

    private async void OSVinculadasWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.CarregarAsync();
    }

    private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        _viewModel.VerDetalhes();
    }

    private void BtnFechar_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}