using System.Windows.Controls;
using SgaAutoEletrica.UI.ViewModels.Configuracoes;

namespace SgaAutoEletrica.UI.Views.Configuracoes;

public partial class UsuariosView : UserControl
{
    private readonly UsuariosViewModel _viewModel;

    public UsuariosView(UsuariosViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
        Loaded += async (s, e) => await _viewModel.CarregarAsync();
    }
}