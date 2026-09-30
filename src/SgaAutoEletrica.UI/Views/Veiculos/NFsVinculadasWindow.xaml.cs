using System.Windows;
using System.Windows.Input;
using MediatR;
using SgaAutoEletrica.UI.ViewModels.Veiculos;

namespace SgaAutoEletrica.UI.Views.Veiculos;

public partial class NFsVinculadasWindow : Window
{
    private readonly NFsVinculadasViewModel _viewModel;

    public NFsVinculadasWindow(IMediator mediator, Guid veiculoId, string placaVeiculo)
    {
        InitializeComponent();
        _viewModel = new NFsVinculadasViewModel(mediator, veiculoId, placaVeiculo);
        DataContext = _viewModel;
        Loaded += NFsVinculadasWindow_Loaded;
    }

    private async void NFsVinculadasWindow_Loaded(object sender, RoutedEventArgs e)
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