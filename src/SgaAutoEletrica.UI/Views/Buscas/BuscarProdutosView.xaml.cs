using System.Windows.Controls;
using SgaAutoEletrica.UI.ViewModels.Buscas;

namespace SgaAutoEletrica.UI.Views.Buscas;

public partial class BuscarProdutosView : UserControl
{
    public BuscarProdutosView(BuscarProdutosViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}