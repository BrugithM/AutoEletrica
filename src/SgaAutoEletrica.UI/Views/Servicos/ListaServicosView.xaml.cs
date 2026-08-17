using System.Windows;
using System.Windows.Controls;
using SgaAutoEletrica.UI.ViewModels.Servicos;

namespace SgaAutoEletrica.UI.Views.Servicos;

public partial class ListaServicosView : UserControl
{
    private readonly ListaServicosViewModel _viewModel;

    public ListaServicosView(ListaServicosViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
        Loaded += ListaServicosView_Loaded;
    }

    private async void ListaServicosView_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.BuscarAsync();
    }
}