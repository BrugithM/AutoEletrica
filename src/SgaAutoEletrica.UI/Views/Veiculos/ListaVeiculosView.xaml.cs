using System.Windows;
using System.Windows.Controls;
using SgaAutoEletrica.UI.ViewModels.Veiculos;

namespace SgaAutoEletrica.UI.Views.Veiculos;

public partial class ListaVeiculosView : UserControl
{
    private readonly ListaVeiculosViewModel _viewModel;

    public ListaVeiculosView(ListaVeiculosViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
        Loaded += ListaVeiculosView_Loaded;
    }

    private async void ListaVeiculosView_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.BuscarAsync();
    }
}