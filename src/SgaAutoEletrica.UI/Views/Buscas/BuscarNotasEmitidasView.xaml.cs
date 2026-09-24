using System.Windows.Controls;
using System.Windows.Input;
using SgaAutoEletrica.UI.ViewModels.Buscas;

namespace SgaAutoEletrica.UI.Views.Buscas;

public partial class BuscarNotasEmitidasView : UserControl
{
    private readonly BuscarNotasEmitidasViewModel _viewModel;

    public BuscarNotasEmitidasView(BuscarNotasEmitidasViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
        Loaded += BuscarNotasEmitidasView_Loaded;
    }

    private async void BuscarNotasEmitidasView_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        await _viewModel.BuscarAsync();
    }

    private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        _viewModel.VerDetalhes();
    }
}