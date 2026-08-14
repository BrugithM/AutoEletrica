using System.Windows;
using System.Windows.Controls;
using SgaAutoEletrica.UI.ViewModels.Clientes;

namespace SgaAutoEletrica.UI.Views.Clientes;

public partial class ListaClientesView : UserControl
{
    private readonly ListaClientesViewModel _viewModel;

    public ListaClientesView(ListaClientesViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
        Loaded += ListaClientesView_Loaded;
    }

    private async void ListaClientesView_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.BuscarAsync();
    }
}