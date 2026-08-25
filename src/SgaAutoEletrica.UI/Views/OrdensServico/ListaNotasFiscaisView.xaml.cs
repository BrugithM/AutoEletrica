using System.Windows;
using System.Windows.Controls;
using SgaAutoEletrica.UI.ViewModels.OrdensServico;

namespace SgaAutoEletrica.UI.Views.OrdensServico;

public partial class ListaNotasFiscaisView : UserControl
{
    private readonly ListaNotasFiscaisViewModel _viewModel;

    public ListaNotasFiscaisView(ListaNotasFiscaisViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
        Loaded += ListaNotasFiscaisView_Loaded;
    }

    private async void ListaNotasFiscaisView_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.CarregarAsync();
    }
}