using System.Windows.Controls;
using SgaAutoEletrica.UI.ViewModels.Buscas;

namespace SgaAutoEletrica.UI.Views.Buscas;

public partial class BuscarNotasEntradaView : UserControl
{
    public BuscarNotasEntradaView(BuscarNotasEntradaViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}