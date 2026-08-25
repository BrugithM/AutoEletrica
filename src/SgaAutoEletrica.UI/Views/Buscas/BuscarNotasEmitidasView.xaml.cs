using System.Windows.Controls;
using SgaAutoEletrica.UI.ViewModels.Buscas;

namespace SgaAutoEletrica.UI.Views.Buscas;

public partial class BuscarNotasEmitidasView : UserControl
{
    public BuscarNotasEmitidasView(BuscarNotasEmitidasViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}