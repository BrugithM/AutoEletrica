using System.Windows.Controls;
using SgaAutoEletrica.UI.ViewModels.Fornecedores;

namespace SgaAutoEletrica.UI.Views.Fornecedores;

public partial class ListaFornecedoresView : UserControl
{
    private readonly ListaFornecedoresViewModel _viewModel;

    public ListaFornecedoresView(ListaFornecedoresViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
        Loaded += ListaFornecedoresView_Loaded;
    }

    private async void ListaFornecedoresView_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        await _viewModel.BuscarAsync();
    }
}