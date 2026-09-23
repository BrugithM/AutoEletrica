using System.Windows.Controls;
using SgaAutoEletrica.UI.ViewModels.Buscas;

namespace SgaAutoEletrica.UI.Views.Buscas;

public partial class BuscarNotasEntradaView : UserControl
{
    private readonly BuscarNotasEntradaViewModel _viewModel;

    public BuscarNotasEntradaView(BuscarNotasEntradaViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
        Loaded += BuscarNotasEntradaView_Loaded;
    }

    private async void BuscarNotasEntradaView_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        await _viewModel.BuscarAsync();
    }
}