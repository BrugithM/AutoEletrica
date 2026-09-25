using System.Windows.Controls;
using System.Windows.Input;
using SgaAutoEletrica.UI.ViewModels.OrdensServico;

namespace SgaAutoEletrica.UI.Views.OrdensServico;

public partial class ListaOSView : UserControl
{
    private readonly ListaOSViewModel _viewModel;

    public ListaOSView(ListaOSViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
        Loaded += ListaOSView_Loaded;
    }

    private async void ListaOSView_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        await _viewModel.BuscarAsync();
    }

    private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        _viewModel.VerDetalhes();
    }
}