using System.Windows;
using System.Windows.Controls;
using SgaAutoEletrica.UI.ViewModels.Pecas;

namespace SgaAutoEletrica.UI.Views.Pecas;

public partial class ListaPecasView : UserControl
{
    private readonly ListaPecasViewModel _viewModel;

    public ListaPecasView(ListaPecasViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
        Loaded += ListaPecasView_Loaded;
    }

    private async void ListaPecasView_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.CarregarCategoriasAsync();
        await _viewModel.BuscarAsync();
    }
}